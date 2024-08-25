using Meadow.Configuration;
using Meadow.DataAccessResolving;
using Meadow.DataTypeMapping;
using Meadow.MySql;
using Meadow.Postgre;
using Meadow.Scaffolding.Attributes;
using Meadow.SQLite.Extensions;
using Meadow.SqlServer;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class DataAccessResolverSuit
{

    private class Context
    {
        private MeadowEngine _engine ;

        public MeadowConfiguration Configuration => new MeadowConfiguration();

        public Context()
        {
            _engine = new MeadowEngine(Configuration);
        }
        public void UseDataAccess(string name)
        {
            
            if (name == "sqlite") _engine.UseSqLite();
            else if (name == "mysql") _engine.UseMySql();
            else if (name == "sqlserver") _engine.UseSqlServer();
            else if (name == "postgre") _engine.UsePostgre();
            
        }
    }

    private readonly ITestOutputHelper _outputHelper;

    public DataAccessResolverSuit(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Theory]
    [InlineData("sqlite")]
    [InlineData("mysql")]
    [InlineData("sqlserver")]
    [InlineData("postgre")]
    public void Should_Resolve_DbTypeNameMapper(string dataAccessName) =>
        Should_Resolve_Type<IDbTypeNameMapper>(dataAccessName);
    
    [Theory]
    [InlineData("sqlite")]
    [InlineData("mysql")]
    [InlineData("sqlserver")]
    [InlineData("postgre")]
    public void Should_Resolve_SqlTranslator(string dataAccessName) =>
        Should_Resolve_Type<IDbTypeNameMapper>(dataAccessName);
    
    [Theory]
    [InlineData("sqlite")]
    [InlineData("mysql")]
    [InlineData("sqlserver")]
    [InlineData("postgre")]
    public void Should_Resolve_ValueTranslator(string dataAccessName) =>
        Should_Resolve_Type<IValueTranslator>(dataAccessName);
    
    
    [Theory]
    [InlineData("sqlite",CommonSnippets.CreateTable)]
    [InlineData("sqlite",CommonSnippets.FullTreeView)]
    [InlineData("sqlite",CommonSnippets.SaveProcedure)]
    [InlineData("mysql",CommonSnippets.CreateTable)]
    [InlineData("mysql",CommonSnippets.FullTreeView)]
    [InlineData("mysql",CommonSnippets.SaveProcedure)]
    public void Should_Resolve_Snippets(string dataAccessName,CommonSnippets snippet)
    {
        var context = new Context();
        
        context.UseDataAccess(dataAccessName);

        var resolver = new DataAccessServiceResolver(context.Configuration);

        var resolvedInstance = resolver.InstantiateSnippet(snippet);

        Assert.NotNull(resolvedInstance);
        
        _outputHelper.WriteLine("DA: {0},Found {1}: {2}",dataAccessName, resolvedInstance.GetType().FullName,snippet);
    }
    
    
    public void Should_Resolve_Type<T>(string dataAccessName) where T : class
    {
        var context = new Context();
        
        context.UseDataAccess(dataAccessName);

        var resolver = new DataAccessServiceResolver(context.Configuration);

        var resolvedInstance = resolver.GetService<T>();

        Assert.NotNull(resolvedInstance);

        Assert.IsAssignableFrom<T>(resolvedInstance);
        
        _outputHelper.WriteLine("DA: {0},Found {1}: {2}",dataAccessName, resolvedInstance.GetType().FullName,typeof(T).Name);
        
    }
    
    
}