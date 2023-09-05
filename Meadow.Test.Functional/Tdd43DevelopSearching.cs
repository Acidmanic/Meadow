using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Meadow.Search.Models;
using Meadow.Search.Services;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional
{
    public class Tdd43DevelopSearching : MeadowMultiDatabaseTestBase
    {
        protected override void SelectDatabase()
        {
            UseMySql();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {

            var fullTreePerson = new Person
            {
                Age = 10,
                Job = J("BoJack", 1000),
                Id = 12,
                Name = "BoJack",
                Surname = "Horseman",
                JobId = 12,
                Addresses = new List<Address>()
                {
                    A(1, 12),
                    A(2, 12),
                    A(3, 12),
                    A(4, 12),
                }
            };
            
            
            var transliterationService = new EnglishTransliterationsService();

            var indexingService = new IndexingService<Person>(transliterationService);

            var corpus = indexingService.GetIndexCorpus(fullTreePerson, true);


            

            var inserted = engine
                .PerformRequest(new InsertRequest<Person>(fullTreePerson))
                .FromStorage.FirstOrDefault();

            var indexed = engine
                .PerformRequest(new IndexEntity<Person,long>(corpus,inserted!.Id))
                .FromStorage.FirstOrDefault();

            Console.WriteLine(corpus);
        }
    }
}