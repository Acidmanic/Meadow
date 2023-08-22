using System;
using System.IO;
using Example.MySql.Requests;
using Examples.Common;
using Meadow;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.MySql;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.MySql
{
    class Program
    {
        public class ConfigurationProvider : IMeadowConfigurationProvider
        {
            public MeadowConfiguration GetConfigurations()
            {
                return new MeadowConfiguration
                {
                    ConnectionString = ExampleConnectionString.GetMySqlConnectionString(),
                    BuildupScriptDirectory = "Scripts",
                    DatabaseFieldNameDelimiter = '_'
                };
            }
        }

        static void Main(string[] args)
        {
            new ConsoleLogger().UseForMeadow();

            // Configure Meadow
            var configuration = new ConfigurationProvider().GetConfigurations();

            // Create Engine:
            var engine = new MeadowEngine(configuration).UseMySql();

            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }


            // Create Database if not exists
            if (!engine.DatabaseExists())
            {
                engine.CreateDatabase();
            }
            // // Create Database if not exists
            // engine.CreateIfNotExist();


            // Setup (update regarding scripts)
            engine.BuildUpDatabase();
            // ready to use
            // Make a request
            var request = new GetAllPersonsFullTreeRequest();

            var response = engine.PerformRequest(request);

            var allPersons = response.FromStorage;

            Console.WriteLine($"Read {allPersons.Count} Persons from database, which where inserted from scripts.");

            allPersons.ForEach(p => Console.WriteLine($"--- {p.Name + " " + p.Surname}"));
        }

        
    }
}