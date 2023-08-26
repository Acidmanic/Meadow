using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Example.Postgre.Filtering.Models;
using Example.Postgre.Filtering.Requests;
using Examples.Common;
using Meadow;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Postgre;
using Meadow.Scaffolding.Macros;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.Postgre.Filtering
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
                    BuildupScriptDirectory = "Scripts",
                    MacroPolicy = MacroPolicies.UpdateScripts,
                    MacroContainingAssemblies = new List<Assembly>
                    {
                        Assembly.GetEntryAssembly(),
                        typeof(IMacro).Assembly,
                        typeof(PostgreDataAccessCore).Assembly
                    }
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
            
            var filter = new FilterQuery();
            filter.FilterName = typeof(Person).FullName;
            filter.Add( new FilterItem
            {
                Key = "Name",
                ValueComparison = ValueComparison.Equal,
                EqualValues = new List<string>{"Mani","Mona","Farshid"}
            });
            var hash = filter.Hash();


            engine.PerformRequest(new RemoveExpiredFilterResultsRequest(1000));

            var foundResult = engine.PerformRequest(
                    new PerformPersonsFilterIfNeededRequest(filter))
                .FromStorage;

            var readResults = engine.PerformRequest(new ReadPersonsChunkRequest(hash, 1, 2))
                .FromStorage;

            
        }
        
    }
}
