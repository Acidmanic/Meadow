using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Meadow.Test.Shared;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class DataBoundSuit
{

    private const Databases Database = Databases.SqLite;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _scriptsDirectory = "SnippetComposedMacroScripts";

    public DataBoundSuit(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    [Fact]
    private void Should_Create_Environment_Without_Exception()
    {
        var environment = CreateEnvironment();

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            
            
        });
    }
    
    
    [Fact]
    private void Should_Return_CorrectRange_ForEachField()
    {
        var environment = CreateEnvironment();

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            
            
        });
    }


    private Environment<EntityBoundDataProvider> CreateEnvironment()
    {
        var environment = new Environment<EntityBoundDataProvider>(_scriptsDirectory,GetType().Name);

        return environment;
    }
}