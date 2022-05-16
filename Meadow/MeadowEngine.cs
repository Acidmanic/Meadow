using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Meadow.Attributes;
using Meadow.Configuration;
using Meadow.Reflection;

namespace Meadow
{
    public class MeadowEngine
    {
        private readonly string _connectionString;

        public MeadowEngine(MeadowConfiguration configuration)
        {
            _connectionString = configuration.ConnectionString;
        }

        public MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(MeadowRequest<TIn, TOut> request)
            where TOut : class, new()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = CreateCommand(request);

                command.Connection = connection;

                connection.Open();

                if (request.ReturnsValue)
                {
                    var records = new List<TOut>();

                    var map =  new TypeFieldMapHelper().GetTableMap<TOut>(FieldNameType.SpOutputParameter);

                    var dataReader = command.ExecuteReader(CommandBehavior.Default);

                    while (dataReader.Read())
                    {
                        var record = new TOut();

                        foreach (var item in map)
                        {
                            var parameterName = item.Key;

                            var value = dataReader[parameterName];

                            item.Value.Setter(record, value);
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

        private SqlCommand CreateCommand<TIn, TOut>(MeadowRequest<TIn, TOut> request)
            where TOut : class, new()
        {
            var command = new SqlCommand(_connectionString)
            {
                CommandType = CommandType.StoredProcedure, CommandText = request.RequestName
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

                var parameterName = "@" + item.Key;

                var parameter = new SqlParameter(parameterName, parameterValue);

                parameter.Direction = ParameterDirection.Input;

                command.Parameters.Add(parameter);
            }

            return command;
        }


        
    }
}