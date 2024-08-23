using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.Extensions;
using Meadow.Requests;
using Meadow.SQLite.CarrierInterceptors;
using Meadow.SQLite.Extensions;
using Meadow.SQLite.ProcedureProcessing;
using Meadow.SQLite.Requests;
using Meadow.SQLite.SqlScriptsGenerators;
using Meadow.Utility;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Meadow.SQLite
{
    public class SqLiteDataAccessCore : MeadowDataAccessCoreBase<IDbCommand, IDataReader>
    {
        private static readonly SemaphoreSlim DatabaseAccessLock = new SemaphoreSlim(1, 1);

        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; set; }

        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; set; }

        public SqLiteDataAccessCore()
        {
            SqliteConnection.ClearAllPools();
            
            AddCarrierInterceptor(new SqLiteCommandInterceptor());
        }


        public override ISqlTranslator ProvideFilterQueryTranslator(MeadowConfiguration configuration)
        {
            return new SqLiteTranslator(configuration);
        }

        protected override IMeadowDataAccessCore InitializeDerivedClass(MeadowConfiguration configuration)
        {
            DataStorageAdapter = new SqLiteStorageAdapter(configuration, Logger);

            StorageCommunication = new SqLiteStorageCommunication();

            return this;
        }

        public override void CreateDatabase(MeadowConfiguration configuration)
        {
            TryDbFile(configuration, file =>
            {
                if (string.IsNullOrWhiteSpace(file)) throw new Exception("Could not find data source file");

                var dbFile = file!;
                
                if (File.Exists(dbFile))
                {
                    throw new Exception("The Database already exists");
                }

                File.Create(dbFile);
                
                configuration.GetSqLiteProcedureManager().DropStoredRoutines();
                
                PerformRequest(new CreateDatabaseRequest(), configuration);
            });
        }

        public override Task CreateDatabaseAsync(MeadowConfiguration configuration)
        {
            return TryDbFileAsync(configuration, async file =>
            {
                if (File.Exists(file))
                {
                    throw new Exception("The Database already exists");
                }
                configuration.GetSqLiteProcedureManager().DropStoredRoutines();

                await PerformRequestAsync(new CreateDatabaseRequest(), configuration);
            });
        }

        public override bool CreateDatabaseIfNotExists(MeadowConfiguration configuration)
        {
            return TryDbFile(configuration, file =>
            {
                if (!File.Exists(file))
                {
                    configuration.GetSqLiteProcedureManager().DropStoredRoutines();
                    
                    PerformRequest(new CreateDatabaseRequest(), configuration);

                    return true;
                }

                return false;
            });
        }

        public override Task<bool> CreateDatabaseIfNotExistsAsync(MeadowConfiguration configuration)
        {
            return TryDbFileAsync(configuration, async file =>
            {
                if (!File.Exists(file))
                {
                    configuration.GetSqLiteProcedureManager().DropStoredRoutines();
                    
                    await PerformRequestAsync(new CreateDatabaseRequest(), configuration);

                    return true;
                }

                return false;
            });
        }

        private void TryDbFile(MeadowConfiguration configuration, Action<string> code)
        {
            TryDbFile(configuration, s =>
            {
                code(s);

                return true;
            });
        }

        private async Task TryDbFileAsync(MeadowConfiguration configuration, Func<string, Task> code)
        {
            await TryDbFileAsync(configuration, async s =>
            {
                await code(s);

                return true;
            });
        }


        private bool TryDbFile(MeadowConfiguration configuration, Func<string, bool> code)
        {
            DatabaseAccessLock.Wait();

            var conInfo = new ConnectionStringParser().Parse(configuration.ConnectionString);

            if (conInfo.ContainsKey("Data Source"))
            {
                var filename = conInfo["Data Source"];

                try
                {
                    var result = code(filename);

                    DatabaseAccessLock.Release();

                    return result;
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "{Exception}", e);
                }
            }

            DatabaseAccessLock.Release();
            return false;
        }

        private async Task<bool> TryDbFileAsync(MeadowConfiguration configuration, Func<string, Task<bool>> code)
        {
            await DatabaseAccessLock.WaitAsync();

            var conInfo = new ConnectionStringParser().Parse(configuration.ConnectionString);

            if (conInfo.ContainsKey("Data Source"))
            {
                var filename = conInfo["Data Source"];

                try
                {
                    var result = await code(filename);

                    DatabaseAccessLock.Release();

                    return result;
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "{Exception}", e);
                }
            }

            DatabaseAccessLock.Release();

            return false;
        }

        public override void DropDatabase(MeadowConfiguration configuration)
        {
            TryDbFile(configuration, file =>
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Error deleting File:{File}\n{Exception}", file, e);
                }

                configuration.GetSqLiteProcedureManager().DropStoredRoutines();
            });
        }

        public override Task DropDatabaseAsync(MeadowConfiguration configuration)
        {
            return TryDbFileAsync(configuration, async file =>
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Error deleting File:{File}\n{Exception}", file, e);
                }

                configuration.GetSqLiteProcedureManager().DropStoredRoutines();
            });
        }

        public override bool DatabaseExists(MeadowConfiguration configuration)
        {
            var exists = false;

            TryDbFile(configuration, file => { exists = File.Exists(file); });

            return exists;
        }

        public override async Task<bool> DatabaseExistsAsync(MeadowConfiguration configuration)
        {
            var exists = false;

            await TryDbFileAsync(configuration, async file => { exists = File.Exists(file); });

            return exists;
        }

        public override List<string> EnumerateProcedures(MeadowConfiguration configuration)
        {
            return SqLiteProcedureManager.Connect(configuration.ConnectionString).ListProcedures();
        }

        public override Task<List<string>> EnumerateProceduresAsync(MeadowConfiguration configuration)
        {
            return Task.Run(() => configuration.GetSqLiteProcedureManager().ListProcedures());
        }

        public override List<string> EnumerateTables(MeadowConfiguration configuration)
        {
            var response = PerformRequest(new EnumerateTablesRequest(), configuration);

            var result = new List<string>();

            if (response.FromStorage != null)
            {
                result.AddRange(response.FromStorage.Select(nr => nr.Name));
            }

            return result;
        }

        public override async Task<List<string>> EnumerateTablesAsync(MeadowConfiguration configuration)
        {
            var response = await PerformRequestAsync(new EnumerateTablesRequest(), configuration);

            var result = new List<string>();

            if (response.FromStorage != null)
            {
                result.AddRange(response.FromStorage.Select(nr => nr.Name));
            }

            return result;
        }

        public override void CreateTable<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);

            var script = new TableCodeGenerator(type, configuration).Generate().Text;

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override Task CreateTableAsync<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);

            var script = new TableCodeGenerator(type, configuration).Generate().Text;

            var request = new SqlRequest(script);

            return PerformRequestAsync(request, configuration);
        }

        public override void CreateInsertProcedure<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);

            var script = new InsertProcedureSnippetGenerator(type, configuration).Generate().Text;

            script = ClearGo(script);

            var procedure = SqLiteProcedure.Parse(script);

            configuration.GetSqLiteProcedureManager().PerformProcedureCreation(procedure);
        }

        public override Task CreateInsertProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.Run(() => CreateInsertProcedure<TModel>(configuration));
        }

        public override void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);

            var script = new ReadSequenceProcedureSnippetGenerator(type, configuration, false, 1, false).Generate()
                .Text;

            script = ClearGo(script);

            var procedure = SqLiteProcedure.Parse(script);

            procedure.Name = configuration.GetNameConvention<TModel>().SelectLastProcedureName;

            configuration.GetSqLiteProcedureManager().PerformProcedureCreation(procedure);
        }

        public override Task CreateLastInsertedProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.Run(() => CreateLastInsertedProcedure<TModel>(configuration));
        }

        public override void CreateReadAllProcedure<TModel>(MeadowConfiguration configuration)
        {
            var script = new ReadProcedureSnippetGeneratorPlainOnly(typeof(TModel), configuration, false).Generate()
                .Text;

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override async Task CreateReadAllProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            var script = new ReadProcedureSnippetGeneratorPlainOnly(typeof(TModel), configuration, false).Generate()
                .Text;

            var request = new SqlRequest(script);

            await PerformRequestAsync(request, configuration);
        }

        private string ClearGo(string script)
        {
            return script
                .Split(new string[] { "GO", "--SPLIT", "go", "Go", "gO" }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                ?.Trim();
        }
    }
}