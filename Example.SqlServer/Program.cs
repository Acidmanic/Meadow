using System;
using System.IO;
using Example.SqlServer.Models;
using Example.SqlServer.Requests;
using Meadow;
using Meadow.Configuration;
using Meadow.Log;
using Meadow.Requests;
using Meadow.Requests.FieldInclusion;
using Meadow.SqlServer;

namespace Example.SqlServer
{
    class Program
    {

        private sealed class InsertJobRequest : MeadowRequest<Job, Job>
        {
            public InsertJobRequest(Job job) : base(true)
            {
                ToStorage = job;
            }

            protected override void OnFieldManipulation(IFieldInclusionMarker<Job> toStorage, IFieldInclusionMarker<Job> fromStorage)
            {
                base.OnFieldManipulation(toStorage, fromStorage);

                toStorage.Exclude(j => j.Id);
            }
        }
        static void Main(string[] args)
        {
            // Configure Meadow
            var configuration = new MeadowConfiguration
            {
                ConnectionString = GetConnectionString(),
                BuildupScriptDirectory = "Scripts"
            };
            // Create Engine:
            var engine = new MeadowEngine(configuration,new ConsoleLogger()).UseSqlServer();

            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }
            // Create Database if not exists
            engine.CreateIfNotExist();
            // Setup (update regarding scripts)
            engine.BuildUpDatabase();
            // ready to use
            // Make a request
            var request = new GetAllPersonsFullTreeRequest();

            var response = engine.PerformRequest(request);

            var allPersons = response.FromStorage;

            Console.WriteLine($"Read {allPersons.Count} Persons from database, which where inserted from scripts.");
            
            allPersons.ForEach(p=> Console.WriteLine($"--- {p.Name + " " + p.Surname}"));
            
        }


        /// <summary>
        /// :D
        /// </summary>
        /// <returns>Sql server password</returns>
        private static string ReadMyVerySecurePasswordFromGitIgnoredFileSoNoOneSeesIt()
        {
            try
            {
                return File.ReadAllText(Path.Join("..","..","..","..", "sa-pass"));
            }
            catch (Exception e)
            {
                throw new Exception("Please create a text file, named 'sa-pass' " +
                                    "containing your password, and put it in the solution directory.");
            }
        }

        private static string GetConnectionString()
        {
            var password = ReadMyVerySecurePasswordFromGitIgnoredFileSoNoOneSeesIt();

            return "Server=localhost;" +
                   "User Id=sa; " +
                   $"Password={password};" +
                   "Database=MeadowTestDb; " +
                   "MultipleActiveResultSets=true";
        }
    }
}