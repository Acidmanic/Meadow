using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore.AdoCoreBase.ConfigurationRequests;
using Meadow.DataTypeMapping;
using Meadow.Reflection.Conventions;
using Meadow.Requests;
using Meadow.Scaffolding.SqlScriptsGenerators;
using Newtonsoft.Json.Serialization;

namespace Meadow.DataAccessCore.AdoCoreBase
{
    public abstract class AdoDataAccessCoreBase : MeadowDataAccessCoreBase<IDbCommand, IDataReader>
    {
        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; set; }

        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; set; }

        protected override IMeadowDataAccessCore InitializeDerivedClass(MeadowConfiguration configuration)
        {
            DataStorageAdapter = new AdoStorageAdapterBase(configuration.DatabaseFieldNameDelimiter,
                DataOwnerNameProvider,
                Logger,
                WriteIntoCommand);

            StorageCommunication = new AdoStorageCommunication(InstantiateCommand, InstantiateConnection);


            return this;
        }

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

            var procedureName = new NameConvention<TModel>().InsertProcedureName;

            var script = GetSqlForCreatingInsertProcedure(procedureName, tableName, parameters);

            var request = new SqlRequest(script);

            PerformRequest(request, configuration);
        }


        public override void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
            var parameters = TypeDatabaseDefinition.FromType<TModel>(GetDbTypeNameMapper());

            var tableName = DataOwnerNameProvider.GetNameForOwnerType(typeof(TModel));

            var procedureName = new NameConvention<TModel>().InsertProcedureName;

            var script = GetSqlForCreatingInsertProcedure(procedureName, tableName, parameters);

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
    }
}