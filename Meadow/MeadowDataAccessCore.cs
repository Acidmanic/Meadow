using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Meadow.Configuration;
using Meadow.Reflection.Mapping;
using Meadow.Requests;

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
                    var dataReader = command.ExecuteReader(CommandBehavior.Default);

                    var writer = ObjectDataWriter.Create<List<TOut>>(request.FullTree);

                    writer.WriteIntoRootObject(dataReader);

                    var records = writer.As<List<TOut>>();

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

            var reader = new ObjectDataReader(storage, request.FullTree);

            var data = reader.ReadRootObject(request.ToStorageMarks);

            foreach (var dataPoint in data)
            {
                var parameter = new SqlParameter("@" + dataPoint.Key, dataPoint.Value ?? DBNull.Value);

                parameter.Direction = ParameterDirection.Input;

                command.Parameters.Add(parameter);
            }

            return command;
        }
    }
}