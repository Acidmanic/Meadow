using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.MySql.ConfigurationRequests;
using Meadow.MySql.Scaffolding.MySqlScriptGenerators;
using Meadow.Requests;


namespace Meadow.MySql
{
    public class MySqlDataAccessCore : MeadowDataAccessCoreBase<IDbCommand, IDataReader>
    {
        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; set; }

        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; set; }

        protected override IMeadowDataAccessCore InitializeDerivedClass(MeadowConfiguration configuration)
        {
            DataStorageAdapter =
                new MySqlStorageAdapter(configuration, logger: Logger);

            StorageCommunication = new MySqlStorageCommunication();

            return this;
        }

        public override void CreateDatabase(MeadowConfiguration configuration)
        {
            var request = new CreateDatabaseRequest();

            PerformConfigurationRequest(request, configuration);
        }

        public override async Task CreateDatabaseAsync(MeadowConfiguration configuration)
        {
            var request = new CreateDatabaseRequest();

            await PerformConfigurationRequestAsync(request, configuration);
        }

        public override bool CreateDatabaseIfNotExists(MeadowConfiguration configuration)
        {
            var request = new CreateIfNotExistRequest();

            var response = PerformConfigurationRequest(request, configuration);

            return response.SingleOrDefault()?.Value ?? false;
        }

        public override async Task<bool> CreateDatabaseIfNotExistsAsync(MeadowConfiguration configuration)
        {
            var request = new CreateIfNotExistRequest();

            var response = await PerformConfigurationRequestAsync(request, configuration);

            return response.SingleOrDefault()?.Value ?? false;
        }

        public override void DropDatabase(MeadowConfiguration configuration)
        {
            var request = new DropDatabaseRequest();

            PerformConfigurationRequest(request, configuration);
        }

        public override async Task DropDatabaseAsync(MeadowConfiguration configuration)
        {
            var request = new DropDatabaseRequest();

            await PerformConfigurationRequestAsync(request, configuration);
        }

        public override bool DatabaseExists(MeadowConfiguration configuration)
        {
            var request = new FindDatabase();

            var config = request.PreConfigure(configuration);

            var response = PerformRequest(request, config);

            if (response.FromStorage != null && response.FromStorage.Count > 0 && response.FromStorage[0] != null)
            {
                return response.FromStorage.Count > 0;
            }

            return false;
        }

        public override async Task<bool> DatabaseExistsAsync(MeadowConfiguration configuration)
        {
            var request = new FindDatabase();

            var config = request.PreConfigure(configuration);

            var response = await PerformRequestAsync(request, config);

            if (response.FromStorage != null && response.FromStorage.Count > 0 && response.FromStorage[0] != null)
            {
                return response.FromStorage.Count > 0;
            }

            return false;
        }

        private List<string> EnumerateDbObject(bool dbProcedureNotTable, MeadowConfiguration configuration)
        {
            var response = dbProcedureNotTable
                ? PerformRequest(new EnumerateProceduresRequest(), configuration)
                : PerformRequest(new EnumerateTablesRequest(), configuration);

            var result = new List<string>();

            if (response.FromStorage != null)
            {
                result = response.FromStorage.Select(n => n.Name).ToList();
            }

            return result;
        }

        private async Task<List<string>> EnumerateDbObjectAsync(bool dbProcedureNotTable,
            MeadowConfiguration configuration)
        {
            var response = dbProcedureNotTable
                ? await PerformRequestAsync(new EnumerateProceduresRequest(), configuration)
                : await PerformRequestAsync(new EnumerateTablesRequest(), configuration);

            var result = new List<string>();

            if (response.FromStorage != null)
            {
                result = response.FromStorage.Select(n => n.Name).ToList();
            }

            return result;
        }

        public override List<string> EnumerateProcedures(MeadowConfiguration configuration)
        {
            return EnumerateDbObject(true, configuration);
        }

        public override async Task<List<string>> EnumerateProceduresAsync(MeadowConfiguration configuration)
        {
            return await EnumerateDbObjectAsync(true, configuration);
        }

        public override List<string> EnumerateTables(MeadowConfiguration configuration)
        {
            return EnumerateDbObject(false, configuration);
        }

        public override async Task<List<string>> EnumerateTablesAsync(MeadowConfiguration configuration)
        {
            return await EnumerateDbObjectAsync(false, configuration);
        }

        public override void CreateTable<TModel>(MeadowConfiguration configuration)
        {
            var script = new TableScriptSnippetGenerator<TModel>(configuration).Generate().Text;

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override async Task CreateTableAsync<TModel>(MeadowConfiguration configuration)
        {
            var script = new TableScriptSnippetGenerator<TModel>(configuration).Generate().Text;

            var request = new SqlRequest(script);

            await PerformRequestAsync(request, configuration);
        }

        public override void CreateInsertProcedure<TModel>(MeadowConfiguration configuration)
        {
            var script = new InsertSnippetProcedureGenerator<TModel>(configuration).Generate().Text;

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override async Task CreateInsertProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);

            var script = new InsertSnippetProcedureGenerator<TModel>(configuration).Generate().Text;

            var request = new SqlRequest(script);

            await PerformRequestAsync(request, configuration);
        }

        public override void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
            var script = new ReadSequenceSnippetProcedureGenerator<TModel>
                (configuration,true, 1, true, false).Generate().Text;

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override async Task CreateLastInsertedProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            var script = new ReadSequenceSnippetProcedureGenerator<TModel>
                (configuration,true, 1, true, false).Generate().Text;

            var request = new SqlRequest(script);

            await PerformRequestAsync(request, configuration);
        }

        public override void CreateReadAllProcedure<TModel>(MeadowConfiguration configuration)
        {
            var script = new ReadSnippetProcedureGenerator<TModel>(configuration,false).Generate().Text;

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override async Task CreateReadAllProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            var script = new ReadSnippetProcedureGenerator<TModel>(configuration,false).Generate().Text;

            var request = new SqlRequest(script);

            await PerformRequestAsync(request, configuration);
        }
    }
}