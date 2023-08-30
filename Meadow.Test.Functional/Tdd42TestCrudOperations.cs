using System;
using System.Linq;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional
{
    public class Tdd42TestCrudOperations : MeadowMultiDatabaseTestBase
    {
        protected override void SelectDatabase()
        {
            UsePostgre();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {
            
            var allPersons = engine.PerformRequest(new ReadAllRequest<Person>()).FromStorage;
            
            if (Persons.Length != allPersons.Count)
            {
                throw new Exception("Unable to read all seeded persons");
            }

            for (int i = 0; i < Persons.Length; i++)
            {
                var s = Persons[i];
                var p = allPersons[i];
                CompareEntities(s,p);
                Log(logger, p);
            }

            logger.LogInformation("[PASS] Insert & Read all were OK");

            var interestIndex = allPersons.Count / 2;
            var personOfInterest = allPersons[interestIndex];

            personOfInterest.Age = 27;
            personOfInterest.Surname += "-Interesting!";

            var updatedPerson = engine
                .PerformRequest(new UpdateRequest<Person>(personOfInterest))
                .FromStorage.FirstOrDefault();

            if (updatedPerson == null)
            {
                throw new Exception("Unable to update entity");
            }
            
            CompareEntities(personOfInterest,updatedPerson);
            
            logger.LogInformation("[PASS] Update is OK");

            var firson = allPersons[0];

            var readFirson = engine
                .PerformRequest(new ReadByIdRequest<Person, long>(firson.Id))
                .FromStorage.FirstOrDefault();
            
            CompareEntities(firson,readFirson);
            
            logger.LogInformation("[PASS] Read By Id Is OK");

            var interestId = personOfInterest.Id;

            var deletionSuccess = engine
                .PerformRequest(new DeleteById<Person, long>(interestId))
                .FromStorage
                .FirstOrDefault();

            if (deletionSuccess == null)
            {
                throw new Exception("Deletion request performance issue.");
            }

            if (deletionSuccess.Success == false)
            {
                throw new Exception("Unable to delete the object");
            }

            var readDeletedPerson = engine
                .PerformRequest(new ReadByIdRequest<Person, long>(interestId))
                .FromStorage
                .FirstOrDefault();

            if (readDeletedPerson != null)
            {
                throw new Exception("The object is not deleted, yet no exception or failure result returned.");
            }

            logger.LogInformation("[PASS] Delete By Id is OK");
            
            logger.LogInformation("{Database}'s Crud operations test has been Passed.",DatabaseName);
        }
    }
}