using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Example.Filtering.Models;
using Example.Filtering.Requests;
using Examples.Common;
using Meadow;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.MySql;
using Meadow.Postgre;
using Meadow.Scaffolding.Macros;
using Meadow.SQLite;
using Meadow.SQLite.Extensions;
using Meadow.SqlServer;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.Filtering
{
    class Program
    {
       
        private static async Task SeedPersons(MeadowEngine engine)
        {
            var personsSeed = new List<Person>
            {
                new Person { Name = "Mani", Surname = "Moayedi", Age = 37, JobId = 1 },
                new Person { Name = "Mona", Surname = "Moayedi", Age = 42, JobId = 2 },
                new Person { Name = "Mina", Surname = "Haddadi", Age = 56, JobId = 3 },
                new Person { Name = "Farshid", Surname = "Moayedi", Age = 63, JobId = 4 },
                new Person { Name = "Farimehr", Surname = "Ayerian", Age = 21, JobId = 5 },
            };

            foreach (var person in personsSeed)
            {
                await engine.PerformRequestAsync(new InsertPersonRequest(person));
            }
        }


        private static MeadowEngine SetupEngineForPostgre()
        {
            // Configure Meadow
            var configuration = new MeadowConfiguration
            {
                ConnectionString = ExampleConnectionString.GetPostgresConnectionString(),
                BuildupScriptDirectory = "Scripts",
                MacroPolicy = MacroPolicies.UpdateScripts,
                MacroContainingAssemblies = new List<Assembly>
                {
                    Assembly.GetEntryAssembly(),
                    typeof(IMacro).Assembly,
                    typeof(PostgreDataAccessCore).Assembly
                }
            };

            // Create Engine:
            var engine = new MeadowEngine(configuration).UsePostgre();

            return engine;
        }
        
        
        private static MeadowEngine SetupEngineForMySql()
        {
            // Configure Meadow
            var configuration = new MeadowConfiguration
            {
                ConnectionString = ExampleConnectionString.GetMySqlConnectionString(),
                BuildupScriptDirectory = "Scripts",
                MacroPolicy = MacroPolicies.UpdateScripts,
                MacroContainingAssemblies = new List<Assembly>
                {
                    Assembly.GetEntryAssembly(),
                    typeof(IMacro).Assembly,
                    typeof(MySqlDataAccessCore).Assembly
                }
            };

            // Create Engine:
            var engine = new MeadowEngine(configuration).UseMySql();

            return engine;
        }
        
        private static MeadowEngine SetupEngineForSqLite()
        {
            // Configure Meadow
            var configuration =  new MeadowConfiguration
            {
                ConnectionString = $"Data Source=meadow-sqlite.db",
                BuildupScriptDirectory = "Scripts",
                MacroPolicy = MacroPolicies.UpdateScripts
            };
                
            configuration.MacroContainingAssemblies.Add(Assembly.GetEntryAssembly());
            configuration.MacroContainingAssemblies.Add(typeof(IMacro).Assembly);
            configuration.MacroContainingAssemblies.Add(typeof(SqLiteDataAccessCore).Assembly);

            // Create Engine:
            var engine = new MeadowEngine(configuration).UseSqLite();

            return engine;
        }
        
        private static MeadowEngine SetupEngineForSqlServer()
        {
            // Configure Meadow
            var configuration = new MeadowConfiguration
            {
                ConnectionString = ExampleConnectionString.GetSqlServerConnectionString(),
                BuildupScriptDirectory = "Scripts",
                MacroPolicy = MacroPolicies.UpdateScripts,
                MacroContainingAssemblies = new List<Assembly>
                {
                    Assembly.GetEntryAssembly(),
                    typeof(IMacro).Assembly,
                    typeof(SqlServerDataAccessCore).Assembly
                }
            };

            // Create Engine:
            var engine = new MeadowEngine(configuration).UseSqlServer();

            return engine;
        }
        
        
        

        static async Task Main(string[] args)
        {
            var engine = SetupEngineForPostgre();

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

            // Seed some Persons!
            await SeedPersons(engine);

            var filter = new FilterQuery();
            filter.EntityType = typeof(Person);
            filter.Add(new FilterItem
            {
                Key = "Name",
                ValueComparison = ValueComparison.Equal,
                EqualValues = new List<string> { "Mani", "Mona", "Farshid" },
                ValueType = typeof(string)
            });
            // filter.Add(new FilterItem
            // {
            //     Key = "Job.IncomeInRials",
            //     ValueComparison = ValueComparison.LargerThan,
            //     Minimum = "1000",
            //     ValueType = typeof(long)
            // });

            engine.PerformRequest(new RemoveExpiredFilterResultsRequest(1000));

            var foundResult = engine.PerformRequest(
                    new PerformPersonsFilterIfNeededRequest(filter))
                .FromStorage;

            var searchId = foundResult.FirstOrDefault()?.SearchId ?? Guid.NewGuid().SearchId();

            var readResults = engine.PerformRequest(new ReadPersonsChunkRequest(searchId, 1, 2))
                .FromStorage;
        }
    }
}