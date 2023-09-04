using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional
{
    public class Tdd41FTestFilterLogicBeingCorrect : MeadowMultiDatabaseTestBase
    {
        protected override void SelectDatabase()
        {
            UseSqlServer();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {
            var filter = new FilterQuery { EntityType = typeof(Person) };
            filter.Add(new FilterItem
            {
                Key = "Age",
                Minimum = "50",
                ValueComparison = ValueComparison.LargerThan,
                ValueType = typeof(int)
            });

            var search = new PerformSearchIfNeededRequest<Person,long>(filter);

            var searchResult = engine.PerformRequest(search).FromStorage;

            if (searchResult.Count != 2)
            {
                throw new Exception($"Search result must be 2 items but it was {searchResult.Count} items");
            }

            var searchId = searchResult[0].SearchId;

            var searchResultPersons = engine.PerformRequest(new ReadChunkRequest<Person>(searchId)).FromStorage;

            if (searchResultPersons.Count != 2)
            {
                throw new Exception("What the hell?");
            }

            var fakeSearchResults = engine.PerformRequest
                (new PerformSearchIfNeededRequest<Person,long>(new FilterQuery { EntityType = typeof(Person) },
                    searchId))
                .FromStorage;

            if (fakeSearchResults.Count != 2)
            {
                throw new Exception("Aha this is the fucking bug!");
            }

            var fakeSearchId = fakeSearchResults[0].SearchId;

            if (fakeSearchId != searchId)
            {
                throw new Exception("Search Id has been changed unexpectedly");
            }

            var fakeSearchPersons = engine.PerformRequest
                (new ReadChunkRequest<Person>(fakeSearchId)).FromStorage;

            foreach (var person in fakeSearchPersons)
            {
                Log(logger, person);
            }

            /*  Test for failing where clause in readChunk */
            filter = new FilterQuery { EntityType = typeof(Person) };
            filter.Add(new FilterItem
            {
                Key = "Age",
                Maximum = "50",
                ValueComparison = ValueComparison.SmallerThan,
                ValueType = typeof(int)
            });

            searchResult = engine.PerformRequest(new PerformSearchIfNeededRequest<Person,long>(filter)).FromStorage;

            if (searchResult.Count != 3)
            {
                throw new Exception($"Search result must be 3 items but it was {searchResult.Count} items");
            }

            searchId = searchResult[0].SearchId;

            searchResultPersons = engine.PerformRequest(new ReadChunkRequest<Person>(searchId)).FromStorage;

            if (searchResultPersons.Count != 3)
            {
                throw new Exception("This probably shows that read chunk is not using " +
                                    "searchId to choose only results related to the searchId.");
            }

            logger.LogInformation("{Database} passed the test for filtering logic", DatabaseName);


            var fieldName = "JobId";
            
            var personsRange = engine
                .PerformRequest(new RangeRequest<Person>(fieldName))
                .FromStorage
                .FirstOrDefault();

            if (personsRange == null)
            {
                throw new Exception("Unable to execute Range procedure");
            }

            if (personsRange.Min.ToString()!.Equals(ReadByHeadlessAddress<Person>(fieldName, Persons.FirstOrDefault()).ToString()) == false)
            {
                throw new Exception("Wrong Minimum value");
            }
            
            if (personsRange.Max.ToString()!.Equals(ReadByHeadlessAddress<Person>(fieldName, Persons.LastOrDefault()).ToString()) == false)
            {
                throw new Exception("Wrong Maximum value");
            }
            
            logger.LogInformation("Range calculated successfully: {Min} - {Max}",
                personsRange.Min, personsRange.Max);

            var existingValues = engine
                .PerformRequest(new ExistingValuesRequest<Person>(fieldName))
                .FromStorage.Select(s => s.Value.ToString());

            if (!existingValues.Any())
            {
                throw new Exception("Unable to read existing values correctly");
            }
            
            foreach (var value in existingValues)
            {
                logger.LogInformation("{FieldName} Can be: {Value}",fieldName,value);
            }

            var fullTreeFilter = new FilterQuery
            {
                EntityType = typeof(Person)
            };
            fullTreeFilter.Add(new FilterItem
            {
                Key = "Job.IncomeInRials",
                Minimum = "300",
                ValueComparison = ValueComparison.LargerThan,
                ValueType = typeof(int)
            });

            var filtreeResults = engine
                .PerformRequest(new PerformSearchIfNeededRequest<Person,long>(fullTreeFilter), true)
                .FromStorage;

            if (filtreeResults == null || filtreeResults.Count == 0)
            {
                throw new Exception("Unable to execute full tree filter request");
            }
            var filtreeSearchId = filtreeResults.FirstOrDefault()?.SearchId;

            var filtreePersons = engine
                .PerformRequest(new ReadChunkRequest<Person>(filtreeSearchId),true)
                .FromStorage;
            
            if (filtreeResults == null || filtreeResults.Count == 0)
            {
                throw new Exception("Unable to execute fulltree read chunk request");
            }
            
            foreach (var person in filtreePersons)
            {
                Log(logger,person);
            }
            
            

            var badFilter = new FilterQuery { EntityType = typeof(Person) };
            badFilter.Add(new FilterItem
            {
                Key = "Job.Title",
                ValueComparison = ValueComparison.Equal,
                ValueType = typeof(string),
                EqualValues = new List<string>{"the-title-that-does-not-exist"}
            });

            var badResult = engine
                .PerformRequest(new PerformSearchIfNeededRequest<Person,long>(badFilter), true)
                .FromStorage;

            if (badResult.Count > 0)
            {
                throw new Exception("bad filter should not find any items");
            }

            var badSearchId = badResult.FirstOrDefault()?.SearchId ?? "";
            
            var badPersons = engine
                .PerformRequest(new ReadChunkRequest<Person>(badSearchId),true)
                .FromStorage;

            if (badPersons.Count > 0)
            {
                throw new Exception("null search id should not fetch any items");
            }
            
            logger.LogInformation("[PASS] Full tree filtering is working");
        }
    }
}