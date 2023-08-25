using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Example.SqLite.Filtering.Models;
using Example.SqLite.Filtering.Requests;
using Meadow;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Scaffolding.Macros;
using Meadow.SQLite;
using Meadow.SQLite.Extensions;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.SqLite.Filtering
{
    class Program
    {
        private const string DatabaseFilePath = "meadow-sqlite.db";

        private class ConfigurationProvider : IMeadowConfigurationProvider
        {
            public MeadowConfiguration GetConfigurations()
            {
                var config =  new MeadowConfiguration
                {
                    ConnectionString = $"Data Source={DatabaseFilePath}",
                    BuildupScriptDirectory = "Scripts",
                    MacroPolicy = MacroPolicies.UpdateScripts
                };
                
                config.MacroContainingAssemblies.Add(Assembly.GetEntryAssembly());
                config.MacroContainingAssemblies.Add(typeof(IMacro).Assembly);
                config.MacroContainingAssemblies.Add(typeof(SqLiteDataAccessCore).Assembly);

                return config;
            }
        }

        
        
        
        
        static async Task Main(string[] args)
        {
            // Configure Meadow
            var configuration = new ConfigurationProvider().GetConfigurations();
            // Set an instance of ILogger (ConsoleLogger) as default logger for Meadow
            new ConsoleLogger().UseForMeadow();
            // Create Engine:
            var engine = new MeadowEngine(configuration).UseSqLite();
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


            var filter = new FilterQuery();
            filter.FilterName = typeof(Person).FullName;
            filter.Add( new FilterItem
            {
                Key = "Name",
                ValueComparison = ValueComparison.Equal,
                EqualValues = new List<string>{"Mani","Mona"}
            });
            var hash = filter.Hash();


            engine.PerformRequest(new RemoveExpiredFilterResultsRequest(1000));

            var foundResult = engine.PerformRequest(
                new PerformPersonsFilterIfNeededRequest(filter))
                .FromStorage;

            var readResults = engine.PerformRequest(new ReadPersonsChunkRequest(hash, 0, 20))
                .FromStorage;
            
            
        }
    }
}
