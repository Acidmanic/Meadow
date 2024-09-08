using Meadow.Configuration;
using Meadow.MySql;
using Meadow.Postgre;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets;
using Meadow.Sql.Extensions;
using Meadow.SQLite;
using Meadow.SqlServer;
using Meadow.Test.Shared;
using Meadow.Utility;
using Xunit.Abstractions;

namespace Meadow.Test.Unit;

public class SnippetToolboxTests
{
    private record ExampleData(Guid Id, string Name, long Length);

    private Dictionary<Databases, string> expectedSelectsByDatabase = new()
    {
        {
            Databases.SqLite, "CREATE PROCEDURE spReadAll(@Name TEXT,@Length INTEGER)\nAS\nSELECT * FROM ExampleDatas;\nGO\n"
        },
        {
            Databases.MySql, "CREATE PROCEDURE spReadAll(IN Name varchar(256),IN Length BIGINT(16))\nBEGIN\nSELECT * FROM ExampleDatas;\nEND;\n"
        },
        {
            Databases.Postgre, "create function \"spReadAll\"(\"par_Name\" TEXT,\"par_Length\" BIGINT) returns setof \"ExampleDatas\" as $$\nbegin\nSELECT * FROM \"ExampleDatas\";\nend;\n$$ language plpgsql;"
        },
        {
            Databases.SqlServer, "CREATE PROCEDURE spReadAll(@Name nvarchar(256),@Length bigint)\nAS\nSELECT * FROM ExampleDatas;\nGO\n"
        }
    };

    private readonly ITestOutputHelper _testOutputHelper;

    public SnippetToolboxTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(Databases.SqLite)]
    [InlineData(Databases.MySql)]
    [InlineData(Databases.Postgre)]
    [InlineData(Databases.SqlServer)]
    public void Should_Produce_CorrectProcedure(Databases database)
    {
        SelectDatabase(database);

        var toolBox = new SnippetToolboxBuilder<ExampleData>(MeadowConfiguration.Null).Build();

        var actual = toolBox.Procedure(RepetitionHandling.Create,
            "spReadAll",
            $"SELECT * FROM {toolBox.SqlTranslator.GetQuoters().QuoteTableName(toolBox.ProcessedType.NameConvention.TableName)};",
            "",toolBox.ProcessedType.NameConvention.TableName,
            toolBox.ProcessedType.Parameters.ToArray());

        var expected = expectedSelectsByDatabase[database];

        _testOutputHelper.WriteLine(actual);
            
        Assert.Equal(expected, actual);
    }


    private void SelectDatabase(Databases database)
    {
        if (database == Databases.Postgre) MeadowEngine.UseDataAccess(new CoreProvider<PostgreDataAccessCore>());
        if (database == Databases.MySql) MeadowEngine.UseDataAccess(new CoreProvider<MySqlDataAccessCore>());
        if (database == Databases.SqLite) MeadowEngine.UseDataAccess(new CoreProvider<SqLiteDataAccessCore>());
        if (database == Databases.SqlServer) MeadowEngine.UseDataAccess(new CoreProvider<SqlServerDataAccessCore>());
    }
}