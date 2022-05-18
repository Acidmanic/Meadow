using System;
using Meadow.Configuration;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Requests;

namespace Meadow.Test.Functional
{
    public class MeadowShouldRetrieveSeedDataFromDatabase : IFunctionalTest
    {
        private static string connectionString =
            "Server=localhost;User Id=sa; Password=never54aga.1n;Database=MeadowScratch; MultipleActiveResultSets=true";

        public void Main()
        {
            new MeadowShouldBuildupTheDatabase().Main();

            var engine = new MeadowEngine(
                new MeadowConfiguration
                {
                    ConnectionString = connectionString,
                    BuildupScriptDirectory = "Scripts"
                });

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
            
            var persons = engine.PerformRequest(new GetAllPersons());

            persons.FromStorage.ForEach(PrintObject);
        }

        private void PrintJob(Job job)
        {
            Console.WriteLine("--------------------------------------");

            Console.WriteLine(nameof(Job.Id) + $@":{job.Id}");
            Console.WriteLine(nameof(Job.Title) + $@":{job.Title}");
            Console.WriteLine(nameof(Job.JobDescription) + $@":{job.JobDescription}");
            Console.WriteLine(nameof(Job.IncomeInRials) + $@":{job.IncomeInRials}");
        }

        private void PrintObject(object o)
        {
            Console.WriteLine("--------------------------------------");

            var type = o.GetType();

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.CanRead)
                {
                    Console.WriteLine(property.Name + ": " + property.GetValue(o));
                }
            }
        }
    }
}