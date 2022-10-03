using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Meadow.Configuration;
using Meadow.DataAccessCore.AdoCoreBase;
using Meadow.DataTypeMapping;
using Npgsql;

namespace Meadow.Postgre
{
    public class PostgreDataAccessCore : AdoDataAccessCoreBase
    {
        private static IDbTypeNameMapper _typeNameMapper = new PostgreDbTypeNameMapper();

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
            return $"CREATE DATABASE \"{databaseName}\" ;";
        }

        protected override string GetSqlForDatabaseExists(string databaseName)
        {
            return $"SELECT true as \"Value\" where (SELECT Count(datname) FROM pg_database WHERE datname = '{databaseName}') > 0;";
        }

        protected override string GetSqlForDroppingDatabase(string databaseName)
        {
            return $"DROP DATABASE \"{databaseName}\"";
        }

        protected override string AsProcedureParameterName(string columnName)
        {
            return "par_" + columnName;
        }

        protected override string GetSqlForCreatingTable(string tableName, TypeDatabaseDefinition parameters)
        {
            var sql = $"CREATE TABLE \"{tableName}\"(";

            parameters = parameters.UpdateForSerialTypes();

            var parameterDefinition = parameters.FieldTypes
                .Select(field => $"\"{field.Key}\" {field.Value.DbTypeName}").ToList();

            if (parameters.HasId)
            {
                parameterDefinition.Add($"PRIMARY KEY(\"{parameters.IdField.ColumnName}\")");
            }

            sql += string.Join(',', parameterDefinition) + ");";

            return sql;
        }


        protected override string GetSqlForCreatingInsertProcedure(
            string procedureName, 
            string tableName,
            TypeDatabaseDefinition parameters)
        {
            var sql = $"create or replace procedure \"{procedureName}\"(";

            parameters = parameters.UpdateForSerialTypes();

            var parameterDefinition = string.Join(',', parameters.FieldTypes
                    .Where(field => field.Value != parameters.IdField)
                    .Select(field =>AsProcedureParameterName(field.Key) + " " + field.Value.DbTypeName));

            sql += parameterDefinition + ") language plpgsql as $$ \n begin \n";

            var columns = string.Join(',', parameters.FieldTypes
                .Where(field => field.Value != parameters.IdField)
                .Select(field => $"\"{field.Key}\""));
            
            var values = string.Join(',', parameters.FieldTypes
                .Where(field => field.Value != parameters.IdField)
                .Select(field => $"\"{AsProcedureParameterName(field.Key)}\""));
            
            sql += $"insert into \"{tableName}\" ({columns}) \n values ({values}) returning * ;\n";

            sql += "commit \nend;$$;";

            return sql;
        }

        protected override string GetSqlForCreatingGetLastInsertedProcedure(
            string procedureName,
            string tableName,
            TypeDatabaseDefinition definition)
        {

            var orderField = definition.IdField;

            if (!definition.HasId)
            {
                orderField = definition.FieldTypes
                    .Where(field=> field.Value.IsNumeric())
                    .Select(field => field.Value)
                    .FirstOrDefault();
            }

            var order = orderField == null ? " " : $" ORDER BY \"{orderField.ColumnName}\" DESC ";
            
            var sql = $"create or replace procedure {procedureName}(" +
                      $") language plpgsql as $$ \n begin \n" +
                      $"SELECT * FROM \"{tableName}\"{order}LIMIT 1 ;\n" +
                      $"commit \nend;$$;";


            return sql;
        }

        protected override string GetSqlForListingAllProcedureNames()
        {
            return "SELECT routine_schema, routine_name FROM information_schema.routines WHERE routine_type = 'PROCEDURE';";
        }

        protected override string GetSqlForListingAllTableNames()
        {
            return "SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE';";
        }

        protected override IDbTypeNameMapper GetDbTypeNameMapper()
        {
            return _typeNameMapper;
        }

    }
}