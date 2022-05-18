using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Meadow.Configuration;
using Meadow.Reflection;

namespace Meadow
{
    internal class MeadowDataAccessCore
    {
        public enum RequestExecutionType
        {
            Procedure = CommandType.StoredProcedure,
            Script = CommandType.Text
        }

        public MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration,
            RequestExecutionType executionType)
            where TOut : class, new()
        {
            request.InitializeBeforeExecution();

            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                var command = CreateCommand(request, configuration, (CommandType) executionType);

                command.Connection = connection;

                connection.Open();

                if (request.ReturnsValue)
                {
                    var records = new List<TOut>();

                    var map = new TypeFieldMapHelper().GetTableMap<TOut>(FieldNameType.SpOutputParameter);

                    var dataReader = command.ExecuteReader(CommandBehavior.Default);

                    List<string> fields = EnumFields(dataReader);

                    while (dataReader.Read())
                    {
                        var record = new TOut();

                        foreach (var item in map)
                        {
                            var parameterName = item.Key;

                            if (fields.Contains(parameterName))
                            {
                                var value = dataReader[parameterName];

                                item.Value.Setter(record, value);
                            }
                        }

                        records.Add(record);
                    }

                    connection.Close();

                    request.FromStorage = records;
                }
                else
                {
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }

            return request;
        }

        private List<string> EnumFields(SqlDataReader dataReader)
        {
            var result = new List<string>();

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                result.Add(dataReader.GetName(i));
            }

            return result;
        }


        private SqlCommand CreateCommand<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration,
            CommandType commandType)
            where TOut : class, new()
        {
            var command = new SqlCommand(configuration.ConnectionString)
            {
                CommandType = commandType,
                CommandText = request.RequestText
            };

            var storage = request.ToStorage;

            if (storage is null)
            {
                return command;
            }

            Dictionary<string, Accessor> map = new TypeFieldMapHelper().GetTableMap<TIn>(FieldNameType.ColumnName);

            foreach (var item in map)
            {
                var parameterValue = item.Value.Getter(storage);

                var parameterName = item.Key;

                if (parameterValue != null
                    && (parameterValue.GetType().IsPrimitive || parameterValue is string || parameterValue is char)
                    && request.IsIncluded(parameterName))
                {
                    var fieldName = "@" + parameterName;

                    var parameter = new SqlParameter(fieldName, parameterValue);

                    parameter.Direction = ParameterDirection.Input;

                    command.Parameters.Add(parameter);
                }
            }

            // figure a way to exclue @Id for insert
            // this inclusion or exclusion is per sp therefor per request
            // add inclusion or exclusion mechanisem in requests
            return command;
        }
    }
}