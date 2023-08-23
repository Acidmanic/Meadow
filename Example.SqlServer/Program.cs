using System;
using System.IO;
using System.Threading.Tasks;
using Example.SqlServer.Requests;
using Examples.Common;
using Meadow;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.SqlServer;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.SqlServer
{
    class Program
    {
        private class ConfigurationProvider : IMeadowConfigurationProvider
        {
            public MeadowConfiguration GetConfigurations()
            {
                return  new MeadowConfiguration
                {
                    ConnectionString = ExampleConnectionString.GetSqlServerConnectionString(),
                    BuildupScriptDirectory = "Scripts"
                };
            }
        }
        
        static void Main(string[] args)
        {
            new ConsoleLogger().UseForMeadow();
            // Configure Meadow
            var configuration = new ConfigurationProvider().GetConfigurations();
            // Create Engine:
            var engine = new MeadowEngine(configuration).UseSqlServer();

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

            allPersons.ForEach(p => Console.WriteLine($"--- {p.Name + " " + p.Surname}"));
            
            
        }

    }
}