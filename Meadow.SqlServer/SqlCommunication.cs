using System;
using System.Data;
using System.Data.SqlClient;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;

namespace Meadow.SqlServer
{
    public class SqlCommunication : IStorageCommunication<IDbCommand, IDataReader>
    {
        public IDbCommand CreateToStorageCarrier(MeadowRequest request, MeadowConfiguration configuration)
        {
            var command = new SqlCommand(configuration.ConnectionString)
            {
                CommandType = request.Execution == RequestExecution.RequestTextIsNameOfRoutine
                    ? CommandType.StoredProcedure
                    : CommandType.Text,
                CommandText = request.RequestText
            };
            return command;
        }

        public void Communicate(IDbCommand carrier, Action<IDataReader> onDataAvailable, MeadowConfiguration configuration, bool returnsValue)
        {
            using var connection = new SqlConnection(configuration.ConnectionString);
            
            carrier.Connection = connection;
                
            connection.Open();

            if (returnsValue)
            {
                var dataReader = carrier.ExecuteReader(CommandBehavior.Default);

                onDataAvailable(dataReader);
            }
            else
            {
                carrier.ExecuteNonQuery();
            }
                
            connection.Close();
        }
    }
}