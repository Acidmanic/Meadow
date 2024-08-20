using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

public class SaveSuit
{

    private readonly ITestOutputHelper _outputHelper;

    public SaveSuit(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }


    [Fact]
    public void Should_Run_NoException()
    {
        var environment = new Environment<PersonsDataProvider>();
        
        
        environment.Perform(Databases.SqLite,new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            
            
        });
    }
}