using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Test.Functional.TestEnvironment;
using Meadow.Test.Functional.TestEnvironment.Utility;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

public class MeadowSqLiteSuit
{
    private readonly ITestOutputHelper _helper;

    public MeadowSqLiteSuit(ITestOutputHelper helper)
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

            _helper.WriteLine("Round: {0} Performed Successfully", i);
        }
    }

    [Fact]
    public void SqLite_ShouldBeAbleTo_CallProcedures_Successively()
    {
        for (int i = 0; i < 5; i++)
        {
            var environment = new Environment<PersonsDataProvider>();
            
            environment.Perform(Databases.SqLite,new LoggerAdapter(_helper.WriteLine), e =>
            {
                var persons = e.FindPaged<Person>();
                
                _helper.WriteLine("Read {0} Persons",persons.FromStorage.Count);
            });

            _helper.WriteLine("Round: {0} Performed Successfully", i);
        }
    }
}