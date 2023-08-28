using System;
using System.Data;
using System.Threading.Tasks;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;
using Microsoft.Data.Sqlite;

namespace Meadow.SQLite
{
    public class SqLiteStorageCommunication : IStorageCommunication<IDbCommand, IDataReader>
    {
        public IDbCommand CreateToStorageCarrier(MeadowRequest request, MeadowConfiguration configuration)
        {
            return new SqliteCommand(request.RequestText);
        }

        public void Communicate(IDbCommand carrier, Action<IDataReader> onDataAvailable,
            MeadowConfiguration configuration, bool returnsValue)
        {
            using (var connection = new SqliteConnection(configuration.ConnectionString))
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

        public Task CommunicateAsync(IDbCommand carrier, Action<IDataReader> onDataAvailable,
            MeadowConfiguration configuration, bool returnsValue)
        {
            return Task.Run(() => Communicate(carrier, onDataAvailable, configuration, returnsValue));
        }
    }
}