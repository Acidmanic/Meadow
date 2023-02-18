using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore.AdoCoreBase;
using Meadow.DataTypeMapping;
using Meadow.SqlServer.Scaffolding.SqlScriptsGenerators;

namespace Meadow.SqlServer
{
    public class SqlServerDataAccessCore:AdoDataAccessCoreBase
    {
        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; set; } 

        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; set; }
        
        
        protected override IDbDataParameter InstantiateParameter()
        {
            return new SqlParameter();
        }

        protected override IDbCommand InstantiateCommand()
        {
            return new SqlCommand();
        }

        protected override IDbConnection InstantiateConnection(MeadowConfiguration configuration)
        {
            return new SqlConnection();
        }

        protected override string GetSqlForCreatingDatabase(string databaseName)
        {
            return $@"CREATE DATABASE {databaseName};";
        }

        protected override string GetSqlForDatabaseExists(string databaseName)
        {
            return
                $@"IF (DB_ID('{databaseName}') IS NOT NULL)
                    select cast(1 as bit) Value 
                ELSE 
                    select cast(0 as bit) Value";
        }

        protected override string GetSqlForDroppingDatabase(string databaseName)
        {
            return $@"DROP DATABASE {databaseName};";
        }

        protected override string GetSqlForCreatingTable(string tableName, TypeDatabaseDefinition parameters)
        {
            var script = new TableScriptGenerator(parameters.CorrespondingType,false).Generate().Text;

            return script;
        }

        protected override string GetSqlForCreatingInsertProcedure(string procedureName, string tableName, TypeDatabaseDefinition parameters)
        {
            var script = new InsertProcedureGenerator(parameters.CorrespondingType).Generate().Text;
            
            script = ClearGo(script);

            return script;
        }

        protected override string GetSqlForCreatingGetLastInsertedProcedure(string procedureName, string tableName,
            TypeDatabaseDefinition definition)
        {
          
            var script = new ReadSequenceProcedureGenerator(definition.CorrespondingType, 1, false)
                .Generate().Text;

            script = ClearGo(script);

            return script;
        }

        protected override string GetSqlForListingAllProcedureNames()
        {
            return "SELECT name 'Name' FROM sys.objects WHERE type_desc = 'SQL_STORED_PROCEDURE';";
        }

        protected override string GetSqlForListingAllTableNames()
        {
            return "SELECT name 'Name' FROM sys.objects WHERE type_desc = 'USER_TABLE';";
        }

        protected override IDbTypeNameMapper GetDbTypeNameMapper()
        {
            return new SqlDbTypeNameMapper();
        }
        
        private string ClearGo(string script)
        {
            return script
                .Split(new string[] {"GO","--SPLIT","go","Go","gO"}, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                ?.Trim();
        }

        protected override void OnClearConnectionPool(IDbConnection connection)
        {
            SqlConnection.ClearPool((SqlConnection)connection);
        }
    }
}