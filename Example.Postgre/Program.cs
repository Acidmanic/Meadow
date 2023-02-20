﻿using System;
using System.Threading.Tasks;
using Example.Postgre.Requests;
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

        private class ConfiProvider : IMeadowConfigurationProvider
        {
            public MeadowConfiguration GetConfigurations()
            {
                return new MeadowConfiguration
                {
                    ConnectionString =  "User ID=postgres;Password=12345;Host=localhost;Port=5432;" +
                                        "Database=MeadowScratch;",
                
                    BuildupScriptDirectory = "Scripts"
                };
            }
        }
        
        
        static async Task Main(string[] args)
        {
            // Configure Meadow
            var configuration = new ConfiProvider().GetConfigurations();
            
            // Create Engine:
            var engine = new MeadowEngine(configuration).UsePostgre();

            new ConsoleLogger().UseForMeadow();

            // if (engine.DatabaseExists())
            // {
            //     engine.DropDatabase();
            // }
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
