using System;
using System.Linq;
using Meadow.Requests.BuiltIn;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional
{
    public class Tdd42TestCrudOperations : PersonUseCaseTestBase
    {
        protected override void SelectDatabase()
        {
            UseMySql();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {
            
            base.Main(engine,logger);
            
            // This method has been added after Tdd43, because after development of search, other filtering 
            // features also might need to index
            Index(engine,Persons);
            
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


            var fulTrees = engine
                .PerformRequest(new ReadAllRequest<Person>(), true)
                .FromStorage;

            // Remember personOfInterest is deleted
            if (fulTrees.Count != 4)
            {
                throw new Exception("Unable to read all items full tree");
            }
            
            logger.LogInformation("[PASS] Read All FullTree is OK");


            var ftrId = 4;
            var ftrSeedIndexId = ftrId -1;
            
            var fullTreeById = engine
                .PerformRequest(new ReadByIdRequest<Person, long>(ftrId), true)
                .FromStorage.FirstOrDefault();

            if (fullTreeById == null)
            {
                throw new Exception("Unable to read fulltree by id");
            }

            if (fullTreeById.Name != Persons[ftrSeedIndexId].Name ||
                fullTreeById.Surname != Persons[ftrSeedIndexId].Surname ||
                fullTreeById.JobId != Persons[ftrSeedIndexId].JobId ||
                fullTreeById.Age != Persons[ftrSeedIndexId].Age ||
                fullTreeById.Addresses == null ||
                fullTreeById.Addresses.Count != ftrId ||
                fullTreeById.Job == null ||
                fullTreeById.JobId != fullTreeById.Job.Id ||
                fullTreeById.Job.Title != Jobs[ftrSeedIndexId].Title ||
                fullTreeById.Job.IncomeInRials != Jobs[ftrSeedIndexId].IncomeInRials ||
                fullTreeById.Job.JobDescription != Jobs[ftrSeedIndexId].JobDescription)
            {
                throw new Exception("FullTree by id has read wrong value");
            }
            logger.LogInformation("[PASS] Read By Id FullTree is OK");

            logger.LogInformation("{Database}'s Crud operations test has been Passed.",DatabaseName);
        }
    }
}