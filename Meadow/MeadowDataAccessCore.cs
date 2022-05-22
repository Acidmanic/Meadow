using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Meadow.Configuration;
using Meadow.Reflection;
using Meadow.Reflection.ObjectTree;

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

                    var flatMap = new TypeAnalyzer().Map<TOut>(request.FullTree);

                    var dataReader = command.ExecuteReader(CommandBehavior.Default);

                    List<string> fields = EnumFields(dataReader);

                    var fieldsToRead =
                        flatMap.FieldNames
                            .Where(field => request.FromStorageMarks.IsIncluded(field) &&
                                            fields.Contains(field)).ToList();

                    while (dataReader.Read())
                    {
                        var record = new TypeAnalyzer().CreateObject<TOut>(request.FullTree);

                        foreach (var field in fieldsToRead)
                        {
                            var parameterName = request.FromStorageMarks.GetPracticalName(field);

                            var value = dataReader[parameterName];
                            if (!(value is DBNull))
                            {    
                                flatMap.Write(field, record, value);
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

            var flatMap = new TypeAnalyzer().Map<TIn>(request.FullTree);

            var fieldsToWrite = flatMap.FieldNames
                .Where(field => request.ToStorageMarks.IsIncluded(field))
                .ToList();

            foreach (var field in fieldsToWrite)
            {
                var parameterValue = flatMap.Read(field, storage);

                var parameterName = "@" + request.ToStorageMarks.GetPracticalName(field);

                var parameter = new SqlParameter(parameterName, parameterValue);

                parameter.Direction = ParameterDirection.Input;

                command.Parameters.Add(parameter);
            }

            return command;
        }
    }
}