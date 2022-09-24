using System;
using Example.SqLite.Requests;
using Meadow;
using Meadow.Configuration;
using Meadow.Postgre;

namespace Example.Postgre
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure Meadow
            var file = "meadow-sqlite.db";
            
            var configuration = new MeadowConfiguration
            {
                ConnectionString =  $"Data Source={file}",
                BuildupScriptDirectory = "Scripts"
            };
            // Create Engine:
            var engine = new MeadowEngine(configuration).UsePostgre();
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
    }
}
