using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Meadow.Attributes;
using Meadow.Configuration;
using Meadow.Configuration.ConfigurationRequests;
using Meadow.Reflection;

namespace Meadow
{
    public class MeadowEngine
    {
        private readonly MeadowConfiguration _configuration;

        public MeadowEngine(MeadowConfiguration configuration)
        {
            _configuration = configuration;
        }


        public MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(MeadowRequest<TIn, TOut> request)
            where TOut : class, new()
        {
            if (request is ConfigurationRequest<TOut> configRequest)
            {
                var result = PerformConfigurationRequest(configRequest);

                configRequest.Result = result;

                return request;
            }

            return PerformRequest(request, _configuration);
        }


        private ConfigurationRequestResult PerformConfigurationRequest<TOut>(ConfigurationRequest<TOut> request)
            where TOut : class, new()
        {
            try
            {
                var config = request.Initialize(_configuration);

                var meadowRequest = PerformRequest(request, config);

                return new ConfigurationRequestResult
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ConfigurationRequestResult
                {
                    Success = false,
                    Exception = e
                };
            }
        }

        private MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new()
        {
            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                var command = CreateCommand(request, configuration);

                command.Connection = connection;

                connection.Open();

                if (request.ReturnsValue)
                {
                    var records = new List<TOut>();

                    var map = new TypeFieldMapHelper().GetTableMap<TOut>(FieldNameType.SpOutputParameter);

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

        public void CreateDatabase()
        {
            PerformRequest(new CreateDatabaseRequest());
        }

        public void DropDatabase()
        {
            PerformRequest(new DropDatabaseRequest());
        }

        public bool DatabaseExists()
        {
            var exists = PerformRequest(new DatabaseExistsRequest());

            return exists.FromStorage[0].Value;
        }

        public void CreateIfNotExist()
        {
            PerformRequest(new CreateIfNotExistRequest());
        }

        private SqlCommand CreateCommand<TIn, TOut>(MeadowRequest<TIn, TOut> request, MeadowConfiguration configuration)
            where TOut : class, new()
        {
            SqlCommand command;

            if (request is ConfigurationRequest<TOut>)
            {
                command = new SqlCommand(configuration.ConnectionString)
                {
                    CommandType = CommandType.Text,
                    CommandText = request.RequestName
                };
            }
            else
            {
                command = new SqlCommand(configuration.ConnectionString)
                {
                    CommandType = CommandType.StoredProcedure, CommandText = request.RequestName
                };
            }

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