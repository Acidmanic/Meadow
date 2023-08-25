using System;
using System.Threading.Tasks;
using Example.Postgre.Requests;
using Examples.Common;
using Meadow;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Postgre;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.Postgre
{
    class Program
    {

        private class PostgreExampleConfigurationProvider : IMeadowConfigurationProvider
        {
            public MeadowConfiguration GetConfigurations()
            {
                return new MeadowConfiguration
                {
                    ConnectionString =  ExampleConnectionString.GetPostgresConnectionString(),
                    BuildupScriptDirectory = "Scripts"
                };
            }
        }
        
        
        static async Task Main(string[] args)
        {
            // Configure Meadow
            var configuration = new PostgreExampleConfigurationProvider().GetConfigurations();
            
            // Create Engine:
            var engine = new MeadowEngine(configuration).UsePostgre();

            new ConsoleLogger().UseForMeadow();

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

            if (response.Failed)
            {
                Console.WriteLine("Failed Reading inserted Persons:");
                Console.WriteLine(response.FailureException);
            }

            Console.WriteLine($"Read {allPersons.Count} Persons from database, which where inserted from scripts.");
            
            allPersons.ForEach(p=> Console.WriteLine($"--- {p.Name + " " + p.Surname}"));
            
        }
        
    }
}
