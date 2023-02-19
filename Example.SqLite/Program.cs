using System;
using System.IO;
using System.Threading.Tasks;
using Example.SqLite.Requests;
using Meadow;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.SQLite;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.SqLite
{
    class Program
    {
        private const string DatabaseFilePath = "meadow-sqlite.db";

        private class ConfigurationProvider : IMeadowConfigurationProvider
        {
            public MeadowConfiguration GetConfigurations()
            {
                return new MeadowConfiguration
                {
                    ConnectionString =  $"Data Source={DatabaseFilePath}",
                    BuildupScriptDirectory = "Scripts"
                };
            }
        }
        static async Task Main(string[] args)
        {

            // Configure Meadow
            var configuration = new ConfigurationProvider().GetConfigurations();
            // Set an instance of ILogger (ConsoleLogger) as default logger for Meadow
            new ConsoleLogger().UseForMeadow();
            // Create Engine:
            var engine = new MeadowEngine(configuration).UseSQLite();
            //Delete database if already exists
            if (await engine.DatabaseExistsAsync())
            {
                await engine.DropDatabaseAsync();
            }
            // Create Database if not exists
            await engine.CreateIfNotExistAsync();
            // Setup (update regarding scripts)
            await engine.BuildUpDatabaseAsync();
            // ready to use
            // Make a request
            var request = new GetAllPersonsFullTreeRequest();

            var response = await engine.PerformRequestAsync(request);

            var allPersons = response.FromStorage;

            Console.WriteLine($"Read {allPersons.Count} Persons from database, which where inserted from scripts.");
            
            allPersons.ForEach(p=> Console.WriteLine($"--- {p.Name + " " + p.Surname}"));
        }
        
    }
}
