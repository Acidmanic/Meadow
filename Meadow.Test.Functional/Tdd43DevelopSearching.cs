using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Search.Services;
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
                    .PerformRequest(new IndexEntity<Person, long>(corpus, person.Id))
                    .FromStorage.FirstOrDefault();
            }

            var fullTreeFilterOver400 = new FilterQueryBuilder<Person>()
                .Where(p => p.Job.IncomeInRials)
                .IsLargerThan("400")
                .Build();
            
            var fullTreeFilterUnder300 = new FilterQueryBuilder<Person>()
                .Where(p => p.Job.IncomeInRials)
                .IsSmallerThan("300")
                .Build();

            var flatFilterOver50 = new FilterQueryBuilder<Person>()
                .Where(p => p.Age)
                .IsLargerThan("50")
                .Build();

            List<Person> Search(bool fullTree, FilterQuery filter, string q)
            {
                var searchTerms = transliterationService.Transliterate(q)
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                Console.WriteLine($"FT:{fullTree.ToString().ToUpper()} Q: {string.Join(',', searchTerms)} - Filter: {filter.ToString()}");

                var searchResults = engine.PerformRequest(new PerformSearchIfNeededRequest<Person, long>(filter, null, searchTerms), fullTree)
                    .FromStorage;

                var searchId = searchResults.FirstOrDefault()?.SearchId ?? Guid.NewGuid().ToString();

                var foundPersons = engine.PerformRequest(new ReadChunkRequest<Person>(searchId), fullTree)
                    .FromStorage;

                foreach (var person in foundPersons)
                {
                    Log(logger, person);
                }

                return foundPersons;
            }

            // Farimehr, Farshid
            var result = Search(false, new FilterQuery(), "far");

            if (result.Count != 2)
            {
                throw new Exception("Invalid search.");
            }
            CompareEntities(Persons[3],result[0]);
            CompareEntities(Persons[4],result[1]);
            
            // Farshid
            result = Search(false, flatFilterOver50, "far");
            
            if (result.Count != 1)
            {
                throw new Exception("Invalid search.");
            }
            CompareEntities(Persons[3],result[0]);
            
            // Farshid
            result = Search(false, flatFilterOver50, null);
            
            if (result.Count != 2)
            {
                throw new Exception("Invalid Filter.");
            }
            CompareEntities(Persons[2],result[0]);
            CompareEntities(Persons[3],result[1]);
            
            result = Search(true, new FilterQuery(), "far");
            
            if (result.Count != 2)
            {
                throw new Exception("Invalid Filter.");
            }

            if (result[0].Addresses.Count == 0 || result[1].Addresses.Count == 1)
            {
                throw new Exception("Problem in full tree");
            }
            
            result = Search(true, fullTreeFilterOver400, "far");
            
            if (result.Count != 1)
            {
                throw new Exception("Invalid Filter.");
            }

            if (result[0].Addresses.Count == 0 )
            {
                throw new Exception("Problem in full tree");
            }
            
            result = Search(true, fullTreeFilterUnder300, null);
            
            if (result.Count != 2)
            {
                throw new Exception("Invalid Filter.");
            }

            if (result[0].Addresses.Count == 0 || result[1].Addresses.Count == 0)
            {
                throw new Exception("Problem in full tree");
            }

            logger.LogInformation("[PASS] filtering + Search OK");

            var dad = new Person
            {
                Age = 63,
                Id = 4,
                JobId = Persons[3].JobId,
                Surname = "Moayedi",
                Name = "Dad"
            };
            engine.PerformRequest(new UpdateRequest<Person>(dad));

            var dadIndexCorpus =  indexingService.GetIndexCorpus(dad, true);
            
            engine.PerformRequest(new IndexEntity<Person, long>(dadIndexCorpus, dad.Id));
            
            result = Search(true, new FilterQuery(), "far");
            
            if (result.Count != 1 ||
                result[0].Name.ToLower()!="farimehr")
            {
                throw new Exception("Invalid Index Update");
            }
            
            if (result[0].Addresses.Count == 0 )
            {
                throw new Exception("Problem in full tree");
            }

            logger.LogInformation("[PASS] indexing updates OK");
        }
    }
}