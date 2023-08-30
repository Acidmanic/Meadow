using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional
{
    public class Tdd40FullTreeAccessNewMethod : MeadowMultiDatabaseTestBase
    {
        protected override void SelectDatabase()
        {
            UseSqlServer();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {
            var allPersons = engine.PerformRequest(new ReadAllRequest<Person>()).FromStorage;

            var allFullTreePersons = engine.PerformRequest(new ReadAllRequest<Person>(), true).FromStorage;
        }
    }
}