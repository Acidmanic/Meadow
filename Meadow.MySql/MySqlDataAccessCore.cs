using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Configuration.ConfigurationRequests;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.MySql.ConfigurationRequests;
using Meadow.Requests;
using Meadow.Scaffolding.SqlScriptsGenerators;
using Meadow.Utility;

namespace Meadow.MySql
{
    public class MySqlDataAccessCore : MeadowDataAccessCoreBase<IDbCommand, IDataReader>
    {
        public override IDataOwnerNameProvider DataOwnerNameProvider { get; } = new PluralDataOwnerNameProvider();

        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; } =
            new MySqlStorageAdapter();

        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; } =
            new MySqlStorageCommunication();

        public MySqlDataAccessCore()
        {
        }
        
        private void PerformConfigurationRequest<TOut>(ConfigurationRequest<TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new()
        {
            try
            {
                var config = request.PreConfigure(configuration);

                PerformRequest(request, config);

            }
            catch (Exception e)
            {
                //
                Console.WriteLine(e);
            }
        }
        
        
        public override void CreateDatabase(MeadowConfiguration configuration)
        {
            var request = new CreateDatabaseRequest();

            PerformConfigurationRequest(request, configuration);
        }

        public override void CreateDatabaseIfNotExists(MeadowConfiguration configuration)
        {
            var request = new CreateIfNotExistRequest();

            PerformConfigurationRequest(request, configuration);
        }

        public override void DropDatabase(MeadowConfiguration configuration)
        {
            var request = new DropDatabaseRequest();

            PerformConfigurationRequest(request, configuration);
        }

        public override bool DatabaseExists(MeadowConfiguration configuration)
        {
            var request = new DatabaseExistsRequest();

            var config = request.PreConfigure(configuration);

            var response = PerformRequest(request, config);

            if (response.FromStorage != null && response.FromStorage.Count > 0 && response.FromStorage[0] != null)
            {
                return response.FromStorage[0].Value;
            }

            return false;
        }

        private List<string> EnumerateDbObject(bool dbProcedureNotTable,MeadowConfiguration configuration)
        {
            var response = dbProcedureNotTable
                ? PerformRequest(new EnumerateProceduresRequest(),configuration)
                : PerformRequest(new EnumerateTablesRequest(),configuration);

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

        public override List<string> EnumerateTables(MeadowConfiguration configuration)
        {
            return EnumerateDbObject(false, configuration);
        }

        public override void CreateTable<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);
            
            var script = new TableScriptGenerator(type).Generate().Text;
            
            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override void CreateInsertProcedure<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);
            
            var script = new InsertProcedureGenerator(type).Generate().Text;
            
            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
            
        }

        public override void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
            var type = typeof(TModel);
            
            var script = new ReadSequenceProcedureGenerator(type, false, 1, false).Generate().Text;
            
            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }
       
    }

}