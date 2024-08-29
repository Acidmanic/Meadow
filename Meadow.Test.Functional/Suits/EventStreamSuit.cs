using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Meadow.Test.Functional.TestEnvironment.Utility;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

public class EventStreamSuit
{
    
    
    private const Databases _database = TestEnvironment.Databases.SqLite;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _scriptsDirectory = "SnippetComposedMacroScripts";

    [Fact]
    public void Should_SetupEngine_NoException()
    {
        var environment = new Environment<PersonsDataProvider>(_scriptsDirectory);
        
        environment.Perform(_database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
           
        });

        
        
    }
}