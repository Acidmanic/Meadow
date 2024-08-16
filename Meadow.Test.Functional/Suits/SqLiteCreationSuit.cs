using Meadow.Test.Functional.TestEnvironment;
using Meadow.Test.Functional.TestEnvironment.Utility;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

public class SqLiteCreationSuit
{

    private readonly ITestOutputHelper _helper;

    public SqLiteCreationSuit(ITestOutputHelper helper)
    {
        _helper = helper;
    }

    [Fact]
    public void SqLite_ShouldBeAbleTo_Create_Successively()
    {

        for (int i = 0; i < 5; i++)
        {
            var setup = new MeadowEngineSetup();
            
            setup.SelectDatabase(Databases.SqLite);

            var engine = setup.CreateEngine();

            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }
            
            engine.CreateDatabase();
            
            engine.BuildUpDatabase();
            
            _helper.WriteLine("Round: {0} Performed Successfully",i);
        }
    }
}