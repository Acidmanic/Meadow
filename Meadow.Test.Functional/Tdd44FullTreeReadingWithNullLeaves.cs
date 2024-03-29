using System;
using System.Linq;
using Meadow.Requests.Common;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional
{
    public class Tdd44FullTreeReadingWithNullLeaves:PersonUseCaseTestBase
    {
        protected override void SelectDatabase()
        {
            UseSqLite();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {
            base.Main(engine, logger);
            
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
                .PerformRequest(new ReadByIdRequest<Person>(inserted.Id))
                .FromStorage.FirstOrDefault();

            if (fullTreePersonRead == null)
            {
                throw new Exception("Problem Reading FullTree");
            }
            
            fullTreePersonRead = engine
                .PerformRequest(new ReadByIdRequest<Person, long>(2))
                .FromStorage.FirstOrDefault();

            if (fullTreePersonRead == null)
            {
                throw new Exception("Problem Reading FullTree");
            }
            
            Log(logger,fullTreePersonRead);
        }
    }
}