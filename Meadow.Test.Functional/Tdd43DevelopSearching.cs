using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.MySql;
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

            


            var allPersons = engine
                .PerformRequest(new ReadAllRequest<Person>(), true)
                .FromStorage;

            foreach (var person in allPersons)
            {
                var corpus = indexingService.GetIndexCorpus(person, true);
                
                var indexed = engine
                    .PerformRequest(new IndexEntity<Person,long>(corpus,person.Id))
                    .FromStorage.FirstOrDefault();    
            }

            var q = "far";

            var searchSegments = transliterationService.Transliterate(q)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            
            var filter = new FilterQueryBuilder<Person>()
                .Where(p => p.Job.IncomeInRials)
                .IsLargerThan("400")
                .Build();
            
            
            var searchResults = engine
                .PerformRequest(new PerformSearchIfNeededRequest<Person, long>
                    (filter, null,searchSegments),true)
                .FromStorage;

            var searchId = searchResults.FirstOrDefault()?.SearchId ?? Guid.NewGuid().ToString();

            var foundPersons = engine
                .PerformRequest(new ReadChunkRequest<Person>(searchId),true)
                .FromStorage;

            foreach (var person in foundPersons)
            {
                Log(logger,person);
            }
            
            
        }
    }
}