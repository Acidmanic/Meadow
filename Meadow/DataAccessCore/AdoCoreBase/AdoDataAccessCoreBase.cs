using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore.AdoCoreBase.ConfigurationRequests;
using Meadow.DataTypeMapping;
using Meadow.Extensions;
using Meadow.Requests;

namespace Meadow.DataAccessCore.AdoCoreBase
{
    public abstract class AdoDataAccessCoreBase : MeadowDataAccessCoreBase<IDbCommand, IDataReader>
    {
        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; set; }

        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; set; }

        protected override IMeadowDataAccessCore InitializeDerivedClass(MeadowConfiguration configuration)
        {
            DataStorageAdapter = new AdoStorageAdapterBase(configuration,Logger,WriteIntoCommand);

            StorageCommunication =
                new AdoStorageCommunication(InstantiateCommand, InstantiateConnection, OnClearConnectionPool,QuotRoutineNames);


            return this;
        }

        protected virtual bool QuotRoutineNames => false;
        
        public override void CreateDatabase(MeadowConfiguration configuration)
        {
            var request = new CreateDatabaseRequest(GetSqlForCreatingDatabase);

            PerformConfigurationRequest(request, configuration);
        }

        public override bool CreateDatabaseIfNotExists(MeadowConfiguration configuration)
        {
            if (!DatabaseExists(configuration))
            {
                CreateDatabase(configuration);

                return true;
            }

            return false;
        }


        public override bool DatabaseExists(MeadowConfiguration configuration)
        {
            var request = new DatabaseExistsRequest(GetSqlForDatabaseExists);

            var response = PerformConfigurationRequest(request, configuration);

            return response.SingleOrDefault()?.Value ?? false;
        }

        public override void DropDatabase(MeadowConfiguration configuration)
        {
            var request = new DropDatabaseRequest(GetSqlForDroppingDatabase);

            PerformConfigurationRequest(request, configuration);
        }

        public override void CreateTable<TModel>(MeadowConfiguration configuration)
        {
            var parameters = TypeDatabaseDefinition.FromType<TModel>(GetDbTypeNameMapper());

            var tableName = DataOwnerNameProvider.GetNameForOwnerType(typeof(TModel));

            var script = GetSqlForCreatingTable(tableName, parameters);

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        public override void CreateInsertProcedure<TModel>(MeadowConfiguration configuration)
        {
            var parameters = TypeDatabaseDefinition.FromType<TModel>(GetDbTypeNameMapper());

            var tableName = DataOwnerNameProvider.GetNameForOwnerType(typeof(TModel));

            var procedureName = configuration.GetNameConvention<TModel>().InsertProcedureName;

            var script = GetSqlForCreatingInsertProcedure(procedureName, tableName, parameters);

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }


        public override void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
            var parameters = TypeDatabaseDefinition.FromType<TModel>(GetDbTypeNameMapper());

            var tableName = DataOwnerNameProvider.GetNameForOwnerType(typeof(TModel));

            var procedureName = configuration.GetNameConvention<TModel>().SelectLastProcedureName;

            var script = GetSqlForCreatingGetLastInsertedProcedure(procedureName, tableName, parameters);

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }

        private List<string> EnumerateDbObject(bool dbProcedureNotTable, MeadowConfiguration configuration)
        {
            var response = dbProcedureNotTable
                ? PerformRequest(new NameResultQuery(GetSqlForListingAllProcedureNames()), configuration)
                : PerformRequest(new NameResultQuery(GetSqlForListingAllTableNames()), configuration);

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
                ? await PerformRequestAsync(new NameResultQuery(GetSqlForListingAllProcedureNames()), configuration)
                : await PerformRequestAsync(new NameResultQuery(GetSqlForListingAllTableNames()), configuration);

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

        private void WriteIntoCommand(DataPoint dataPoint, IDbCommand command)
        {
            var parameter = InstantiateParameter();

            parameter.ParameterName = AsProcedureParameterName(dataPoint.Identifier);

            parameter.Value = dataPoint.Value ?? DBNull.Value;

            parameter.Direction = ParameterDirection.Input;

            command.Parameters.Add(parameter);
        }

        protected virtual string AsProcedureParameterName(string columnName)
        {
            return columnName;
        }

        protected abstract IDbDataParameter InstantiateParameter();

        protected abstract IDbCommand InstantiateCommand();

        protected virtual void OnClearConnectionPool(IDbConnection connection)
        {
        }

        protected abstract IDbConnection InstantiateConnection(MeadowConfiguration configuration);

        protected abstract string GetSqlForCreatingDatabase(string databaseName);
        protected abstract string GetSqlForDatabaseExists(string databaseName);

        protected abstract string GetSqlForDroppingDatabase(string databaseName);

        protected abstract string GetSqlForCreatingTable(string tableName,
            TypeDatabaseDefinition parameters);

        protected abstract string GetSqlForCreatingInsertProcedure(string procedureName, string tableName,
            TypeDatabaseDefinition parameters);

        protected abstract string GetSqlForCreatingGetLastInsertedProcedure(string procedureName, string tableName,
            TypeDatabaseDefinition definition);


        protected abstract string GetSqlForListingAllProcedureNames();

        protected abstract string GetSqlForListingAllTableNames();
        protected abstract IDbTypeNameMapper GetDbTypeNameMapper();


        public override async Task CreateDatabaseAsync(MeadowConfiguration configuration)
        {
            var request = new CreateDatabaseRequest(GetSqlForCreatingDatabase);

            await PerformConfigurationRequestAsync(request, configuration);
        }

        public override async Task<bool> CreateDatabaseIfNotExistsAsync(MeadowConfiguration configuration)
        {
            if (!await DatabaseExistsAsync(configuration))
            {
                await CreateDatabaseAsync(configuration);

                return true;
            }

            return false;
        }

        public override async Task DropDatabaseAsync(MeadowConfiguration configuration)
        {
            var request = new DropDatabaseRequest(GetSqlForDroppingDatabase);

            await PerformConfigurationRequestAsync(request, configuration);
        }

        public override async Task<bool> DatabaseExistsAsync(MeadowConfiguration configuration)
        {
            var request = new DatabaseExistsRequest(GetSqlForDatabaseExists);

            var response = await PerformConfigurationRequestAsync(request, configuration);

            return response.SingleOrDefault()?.Value ?? false;
        }

        public override async Task<List<string>> EnumerateProceduresAsync(MeadowConfiguration configuration)
        {
            return await EnumerateDbObjectAsync(true, configuration);
        }

        public override async Task<List<string>> EnumerateTablesAsync(MeadowConfiguration configuration)
        {
            return await EnumerateDbObjectAsync(true, configuration);
        }

        public override async Task CreateTableAsync<TModel>(MeadowConfiguration configuration)
        {
            var parameters = TypeDatabaseDefinition.FromType<TModel>(GetDbTypeNameMapper());

            var tableName = DataOwnerNameProvider.GetNameForOwnerType(typeof(TModel));

            var script = GetSqlForCreatingTable(tableName, parameters);

            var request = new SqlRequest(script);

            await PerformRequestAsync(request, configuration);
        }

        public override async Task CreateInsertProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            var parameters = TypeDatabaseDefinition.FromType<TModel>(GetDbTypeNameMapper());

            var tableName = DataOwnerNameProvider.GetNameForOwnerType(typeof(TModel));

            var procedureName = configuration.GetNameConvention<TModel>().InsertProcedureName;
            
            var script = GetSqlForCreatingInsertProcedure(procedureName,tableName,parameters);

            
            var request = new SqlRequest(script);

            await PerformRequestAsync(request, configuration);
        }

        public override async Task CreateLastInsertedProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            var parameters = TypeDatabaseDefinition.FromType<TModel>(GetDbTypeNameMapper());

            var tableName = DataOwnerNameProvider.GetNameForOwnerType(typeof(TModel));

            var procedureName = configuration.GetNameConvention<TModel>().SelectLastProcedureName;

            var script = GetSqlForCreatingGetLastInsertedProcedure(procedureName, tableName, parameters);

            var request = new SqlRequest(script);

            await PerformRequestAsync(request, configuration);
        }
    }
}