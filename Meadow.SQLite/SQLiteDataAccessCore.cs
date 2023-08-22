using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.Requests;
using Meadow.SQLite.CarrierInterceptors;
using Meadow.SQLite.ProcedureProcessing;
using Meadow.SQLite.Requests;
using Meadow.SQLite.SqlScriptsGenerators;
using Meadow.Utility;
using Microsoft.Extensions.Logging;

namespace Meadow.SQLite
{
    public class SqLiteDataAccessCore : MeadowDataAccessCoreBase<IDbCommand, IDataReader>
    {
        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; set; }

        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; set; }

        public SqLiteDataAccessCore()
        {
            AddCarrierInterceptor(new SQLiteCommandInterceptor());
        }


        public override IFilterQueryTranslator ProvideFilterQueryTranslator()
        {
            return new SqLiteFilterQueryTranslator();
        }

        protected override IMeadowDataAccessCore InitializeDerivedClass(MeadowConfiguration configuration)
        {
            DataStorageAdapter =
                new SqLiteStorageAdapter(
                    configuration.DatabaseFieldNameDelimiter,
                    DataOwnerNameProvider,
                    Logger);

            StorageCommunication = new SQLiteStorageCommunication();

            return this;
        }

        public override void CreateDatabase(MeadowConfiguration configuration)
        {
            TryDbFile(configuration, file =>
            {
                if (File.Exists(file))
                {
                    throw new Exception("The Database already exists");
                }

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

                await PerformRequestAsync(new CreateDatabaseRequest(), configuration);
            });
        }

        public override bool CreateDatabaseIfNotExists(MeadowConfiguration configuration)
        {
            return TryDbFile(configuration, file =>
            {
                if (!File.Exists(file))
                {
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
            var conInfo = new ConnectionStringParser().Parse(configuration.ConnectionString);

            if (conInfo.ContainsKey("Data Source"))
            {
                var filename = conInfo["Data Source"];

                SqLiteProcedureManager.Instance.AssignDatabase(filename);

                try
                {
                    return code(filename);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "{Exception}", e);
                }
            }

            return false;
        }

        private async Task<bool> TryDbFileAsync(MeadowConfiguration configuration, Func<string, Task<bool>> code)
        {
            var conInfo = new ConnectionStringParser().Parse(configuration.ConnectionString);

            if (conInfo.ContainsKey("Data Source"))
            {
                var filename = conInfo["Data Source"];

                SqLiteProcedureManager.Instance.AssignDatabase(filename);

                try
                {
                    return await code(filename);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "{Exception}", e);
                }
            }

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
            return SqLiteProcedureManager.Instance.ListProcedures();
        }

        public override Task<List<string>> EnumerateProceduresAsync(MeadowConfiguration configuration)
        {
            return Task.Run(() => SqLiteProcedureManager.Instance.ListProcedures());
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

            var script = new TableScriptGenerator(type).Generate().Text;

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override Task CreateTableAsync<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);

            var script = new TableScriptGenerator(type).Generate().Text;

            var request = new SqlRequest(script);

            return PerformRequestAsync(request, configuration);
        }

        public override void CreateInsertProcedure<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);

            var script = new InsertProcedureGenerator(type).Generate().Text;

            script = ClearGo(script);

            var procedure = SqLiteProcedure.Parse(script);

            SqLiteProcedureManager.Instance.AddProcedure(procedure);
        }

        public override Task CreateInsertProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.Run(() => CreateInsertProcedure<TModel>(configuration));
        }

        public override void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);

            var script = new ReadSequenceProcedureGenerator(type, false, 1, false).Generate().Text;

            script = ClearGo(script);

            var procedure = SqLiteProcedure.Parse(script);

            SqLiteProcedureManager.Instance.AddProcedure(procedure);
        }

        public override Task CreateLastInsertedProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.Run(() => CreateLastInsertedProcedure<TModel>(configuration));
        }

        public override void CreateReadAllProcedure<TModel>(MeadowConfiguration configuration)
        {
            var script = new ReadProcedureGenerator(typeof(TModel), false).Generate().Text;

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override async Task CreateReadAllProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            var script = new ReadProcedureGenerator(typeof(TModel), false).Generate().Text;

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