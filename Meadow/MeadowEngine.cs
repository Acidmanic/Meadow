using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Meadow.Attributes;
using Meadow.Configuration;

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

                    var map = GetTableMap<TOut>(FieldNameType.SpOutputParameter);

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

            Dictionary<string, Accessor> map = GetTableMap<TIn>(FieldNameType.ColumnName);

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


        private Dictionary<string, Accessor> GetTableMap<T>(FieldNameType fieldNameType)
        {
            var map = new Dictionary<string, Accessor>();

            var type = typeof(T);

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                string mappedName = GetMappedName(property, fieldNameType);

                var accessor = new Accessor();

                accessor.Getter = obj => property.GetValue(obj);

                accessor.Setter = (obj, value) => property.SetValue(obj, value);

                map.Add(mappedName, accessor);
            }

            //TODO: Here you can cache a map of string, func<object,object> for 
            //TODO: each type, where the second object is the storage itself to be passed
            return map;
        }

        private string GetMappedName(PropertyInfo property, FieldNameType fieldNameType)
        {
            string name = property.Name;
            //TODO: Put naming conventions here   

            List<FieldAttribute> attributes = new List<FieldAttribute>();

            if (fieldNameType == FieldNameType.ColumnName)
            {
                attributes.AddRange(property.GetCustomAttributes<ColumnAttribute>());
            }
            else
            {
                attributes.AddRange(property.GetCustomAttributes<ParameterAttribute>());
            }

            if (attributes.Count > 0)
            {
                name = attributes.Last().Name;
            }

            return name;
        }
    }
}