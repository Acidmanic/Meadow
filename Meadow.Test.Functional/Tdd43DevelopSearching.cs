using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Requests.Common;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Search.Services;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace Meadow.Test.Functional
{
    public class Tdd43DevelopSearching : PersonUseCaseTestBase
    {
        protected override void SelectDatabase()
        {
            UseSqLite();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {
            base.Main(engine,logger);
            
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


            List<Person> Search(bool fullTree, FilterQuery filter, string q, params OrderTerm[] orders)
            {
                Console.WriteLine("------------------------------------------------");

                var searchTerms = transliterationService.Transliterate(q)
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var ordersString = string.Join(", ",
                    orders.Select(o => o.Key + (o.Sort == OrderSort.Ascending ? "|^|" : "|v|")));

                Console.WriteLine(
                    $"FT:{fullTree.ToString().ToUpper()} Q: {string.Join(',', searchTerms)} - Filter: {filter.ToString()}" +
                    $" - Order: {ordersString}");

                Console.WriteLine("------------------------------------------------");
                var searchResults = engine
                    .PerformRequest(new PerformSearchIfNeededRequest<Person, long>(
                        filter, null, searchTerms, orders), fullTree)
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

            CompareEntities(Persons[3], result[0]);
            CompareEntities(Persons[4], result[1]);

            // Farshid
            result = Search(false, flatFilterOver50, "far");

            if (result.Count != 1)
            {
                throw new Exception("Invalid search.");
            }

            CompareEntities(Persons[3], result[0]);

            // Farshid
            result = Search(false, flatFilterOver50, null);

            if (result.Count != 2)
            {
                throw new Exception("Invalid Filter.");
            }

            CompareEntities(Persons[2], result[0]);
            CompareEntities(Persons[3], result[1]);

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

            if (result[0].Addresses.Count == 0)
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

            var dadIndexCorpus = indexingService.GetIndexCorpus(dad, true);

            engine.PerformRequest(new IndexEntity<Person, long>(dadIndexCorpus, dad.Id));

            result = Search(true, new FilterQuery(), "far");

            if (result.Count != 1 ||
                result[0].Name.ToLower() != "farimehr")
            {
                throw new Exception("Invalid Index Update");
            }

            if (result[0].Addresses.Count == 0)
            {
                throw new Exception("Problem in full tree");
            }

            logger.LogInformation("[PASS] indexing updates OK");

            var orderByAge = new OrderTerm[] { new OrderTerm { Key = "Age" } };

            var orderSurnameAscAgeDesc = new OrderSetBuilder<Person>()
                .OrderAscendingBy(p => p.Surname)
                .OrderDescendingBy(p => p.Age).Build();
            
            result = Search(false, new FilterQuery(), null, orderByAge);

            if (result.Count != Persons.Length)
            {
                throw new Exception("Invalid search by order");
            }

            if (result[0].Name != Persons[^1].Name)
            {
                throw new Exception("Invalid Order");
            }


            result = Search(false, new FilterQuery(), null, orderSurnameAscAgeDesc);

            string[] expectedSurnames = { "Ayerian", "Haddadi", "Moayedi", "Moayedi", "Moayedi" };
            int[] expectedAges = { 21, 56, 63, 42, 37 };

            for (int i = 0; i < 4; i++)
            {
                if (result[i].Surname != expectedSurnames[i])
                {
                    throw new Exception("Invalid surname - invalid order");
                }

                if (result[i].Age != expectedAges[i])
                {
                    throw new Exception("Invalid Age - invalid order");
                }
            }

            result = Search(true, new FilterQuery(), null, orderSurnameAscAgeDesc);

            for (int i = 0; i < 4; i++)
            {
                if (result[i].Surname != expectedSurnames[i])
                {
                    throw new Exception("Invalid surname - invalid order");
                }

                if (result[i].Age != expectedAges[i])
                {
                    throw new Exception("Invalid Age - invalid order");
                }
            }

            var orderSurnameAscIncomeDesc = new OrderSetBuilder<Person>()
                .OrderAscendingBy(p => p.Surname)
                .OrderDescendingBy(p => p.Job.IncomeInRials)
                .Build();


            result = Search(true, new FilterQuery(), null, orderSurnameAscIncomeDesc);

            int[] expectedIncomes = { 500, 300, 400, 200, 100 };

            for (int i = 0; i < 4; i++)
            {
                if (result[i].Surname != expectedSurnames[i])
                {
                    throw new Exception("Invalid surname - invalid order");
                }

                if (result[i].Job.IncomeInRials != expectedIncomes[i])
                {
                    throw new Exception("Invalid IncomeInRials - invalid order");
                }
            }
            logger.LogInformation("[PASS] Ordering OK");
        }
    }
}