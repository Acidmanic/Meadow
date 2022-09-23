using System;
using System.Collections.Generic;
using System.Data;
using Meadow.Configuration;
using Meadow.DataAccessCore.AdoCoreBase;
using Meadow.DataTypeMapping;
using Npgsql;

namespace Meadow.Postgre
{
    public class PostgreDataAccessCore:AdoDataAccessCoreBase
    {
        protected override IDbDataParameter InstantiateParameter()
        {
            return new NpgsqlParameter();
        }

        protected override IDbCommand InstantiateCommand()
        {
            return new NpgsqlCommand();
        }

        protected override IDbConnection InstantiateConnection(MeadowConfiguration configuration)
        {
            return new NpgsqlConnection();
        }

        protected override string GetSqlForCreatingDatabase(string databaseName)
        {
            return $@"CREATE DATABASE {databaseName}";
        }

        protected override string GetSqlForDatabaseExists(string databaseName)
        {
            return $@"SELECT true where exists (SELECT FROM pg_database WHERE datname = {databaseName})";
        }

        protected override string GetSqlForDroppingDatabase(string databaseName)
        {
            return $"DROP DATABASE {databaseName}";
        }

        protected override string GetSqlForCreatingTable(string tableName, Dictionary<string, FieldType> parameters)
        {
            throw new NotImplementedException();
        }

        protected override string GetSqlForCreatingInsertProcedure(string procedureName, string tableName, Dictionary<string, FieldType> parameters)
        {
            throw new NotImplementedException();
        }

        protected override string GetSqlForCreatingGetLastInsertedProcedure(string procedureName, string tableName, Dictionary<string, FieldType> parameters)
        {
            throw new NotImplementedException();
        }

        protected override string GetSqlForListingAllProcedureNames()
        {
            throw new NotImplementedException();
        }

        protected override string GetSqlForListingAllTableNames()
        {
            throw new NotImplementedException();
        }

        protected override IDbTypeNameMapper GetDbTypeNameMapper()
        {
            throw new NotImplementedException();
        }
    }
}