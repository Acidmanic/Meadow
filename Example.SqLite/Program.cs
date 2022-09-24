using System;
using System.IO;
using Example.SqLite.Requests;
using Meadow;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.SQLite;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.SqLite
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
            // Set an instance of ILogger (ConsoleLogger) as default logger for Meadow
            new ConsoleLogger().UseForMeadow();
            // Create Engine:
            var engine = new MeadowEngine(configuration).UseSQLite();
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
