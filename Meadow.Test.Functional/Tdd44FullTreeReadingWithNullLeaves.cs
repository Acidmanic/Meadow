using System;
using System.Linq;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional
{
    public class Tdd44FullTreeReadingWithNullLeaves:MeadowMultiDatabaseTestBase
    {
        protected override void SelectDatabase()
        {
            UseMySql();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {
            var person = new Person
            {
                Name = "Loner",
                Surname = "NoONe",
                Age = 123,
                JobId = 0
            };

            var inserted = engine.PerformRequest(new InsertRequest<Person>(person))
                .FromStorage.FirstOrDefault();

            if (inserted == null)
            {
                throw new Exception("Problem inserting");
            }

            var fullTreePersonRead = engine
                .PerformRequest(new ReadByIdRequest<Person, long>(inserted.Id), true)
                .FromStorage.FirstOrDefault();

            if (fullTreePersonRead == null)
            {
                throw new Exception("Problem Reading FullTree");
            }
            
            Log(logger,fullTreePersonRead);
        }
    }
}