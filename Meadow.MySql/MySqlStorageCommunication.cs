using System;
using System.Data;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;
using MySql.Data.MySqlClient;

namespace Meadow.MySql
{
    public class MySqlStorageCommunication:IStorageCommunication<IDbCommand,IDataReader>
    {
        
        public IDbCommand CreateToStorageCarrier(MeadowRequest request, MeadowConfiguration configuration)
        {
            return new MySqlCommand(request.RequestText);
        }

        public void Communicate(IDbCommand carrier, Action<IDataReader> onDataAvailable, MeadowConfiguration configuration, bool returnsValue)
        {
            using (var connection = new MySqlConnection(configuration.ConnectionString))
            {
                carrier.Connection = connection;

                connection.Open();

                if (returnsValue)
                {
                    var reader = carrier.ExecuteReader();

                    onDataAvailable(reader);
                }
                else
                {
                    carrier.ExecuteNonQuery();
                }
            }
        }
    }
}