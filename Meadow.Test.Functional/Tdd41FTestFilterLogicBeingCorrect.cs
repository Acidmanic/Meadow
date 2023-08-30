using System;
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
            UseMySql();
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

            var search = new PerformSearchIfNeededRequest<Person>(filter);

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
                (new PerformSearchIfNeededRequest<Person>(new FilterQuery { EntityType = typeof(Person) },
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

            searchResult = engine.PerformRequest(new PerformSearchIfNeededRequest<Person>(filter)).FromStorage;

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

            if (personsRange.Min.Equals(ReadByHeadlessAddress<Person>(fieldName, Persons.FirstOrDefault())) == false)
            {
                throw new Exception("Wrong Minimum value");
            }
            
            if (personsRange.Max.Equals(ReadByHeadlessAddress<Person>(fieldName, Persons.LastOrDefault())) == false)
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
            
        }
    }
}