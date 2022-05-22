using System;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Requests;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd004MeadowShouldRetrieveSeedDataFromDatabase : MeadowFunctionalTest
    {
        public Tdd004MeadowShouldRetrieveSeedDataFromDatabase() : base("MeadowScratch")
        {
        }

        public override void Main()
        {
            new Tdd002MeadowShouldBuildupTheDatabase().Main();

            var engine = CreateEngine();

            var jobs = engine.PerformRequest(new GetAllJobsRequest());

            jobs.FromStorage.ForEach(PrintObject);

            var insertRequest = new InsertPerson
            {
                ToStorage = new Person
                {
                    Age = 12,
                    Job = new Job(),
                    Name = "Artin",
                    Surname = "Khadivz",
                    JobId = 1
                }
            };

            var insertResult = engine.PerformRequest(insertRequest);

            var persons = engine.PerformRequest(new GetAllPersonsRequest());

            persons.FromStorage.ForEach(PrintObject);

        }
    }
}