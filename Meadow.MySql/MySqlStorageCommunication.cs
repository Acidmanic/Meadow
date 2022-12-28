using System;
using System.Data;
using System.Threading.Tasks;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.MySql.Comments;
using Meadow.Requests;
using MySql.Data.MySqlClient;

namespace Meadow.MySql
{
    public class MySqlStorageCommunication:IStorageCommunication<IDbCommand,IDataReader>
    {
        
        public IDbCommand CreateToStorageCarrier(MeadowRequest request, MeadowConfiguration configuration)
        {
            

            if (request.Execution == RequestExecution.RequestTextIsNameOfRoutine)
            {
                var procedureCarrier =  new MySqlCommand(request.RequestText);
                
                procedureCarrier.CommandType = CommandType.StoredProcedure;
            
                return procedureCarrier;
            }
            
            var textCarrier =  new MySqlCommand(request.RequestText?.ClearMySqlComments());
            
            textCarrier.CommandType = CommandType.Text;

            return textCarrier;
        }

        public void Communicate(IDbCommand carrier, Action<IDataReader> onDataAvailable, MeadowConfiguration configuration, bool returnsValue)
        {

            if (string.IsNullOrWhiteSpace(carrier.CommandText))
            {
                return;
            }
            
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

        public Task CommunicateAsync(IDbCommand carrier, Action<IDataReader> onDataAvailable, MeadowConfiguration configuration, bool returnsValue)
        {
            return Task.Run(() =>
            {
                Communicate(carrier, onDataAvailable, configuration, returnsValue);
            });
        }
    }
}