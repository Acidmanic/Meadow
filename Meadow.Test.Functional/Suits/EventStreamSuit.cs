using Meadow.Test.Functional.TestEnvironment;
using Meadow.Test.Functional.TestEnvironment.Utility;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

public class EventStreamSuit
{
    
    
    private const Databases _database = TestEnvironment.Databases.SqLite;
    private readonly ITestOutputHelper _testOutputHelper;


    [Fact]
    public void Should_SetupEngine_NoException()
    {
        var setup = new MeadowEngineSetup();
        
        setup.SelectDatabase(_database);

        var engine = setup.CreateEngine();
        
    }
}