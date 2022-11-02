using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;

namespace Meadow.DataAccessCore.AdoCoreBase
{
    internal class AdoStorageCommunication : IStorageCommunication<IDbCommand, IDataReader>
    {
        private readonly Func<IDbCommand> _commandFactory;
        private readonly Func<MeadowConfiguration, IDbConnection> _connectionFactory;
        private readonly Action<IDbConnection> _clearPoolAction;

        public AdoStorageCommunication(Func<IDbCommand> commandFactory,
            Func<MeadowConfiguration, IDbConnection> connectionFactory, Action<IDbConnection> clearPoolAction)
        {
            _commandFactory = commandFactory;
            _connectionFactory = connectionFactory;
            _clearPoolAction = clearPoolAction;
        }


        public IDbCommand CreateToStorageCarrier(MeadowRequest request, MeadowConfiguration configuration)
        {
            var carrier = _commandFactory.Invoke();

            carrier.CommandText = request.RequestText;

            if (request.Execution == RequestExecution.RequestTextIsNameOfRoutine)
            {
                carrier.CommandType = CommandType.StoredProcedure;
            }
            else
            {
                carrier.CommandType = CommandType.Text;
            }

            return carrier;
        }

        public void Communicate(IDbCommand carrier, Action<IDataReader> onDataAvailable,
            MeadowConfiguration configuration, bool returnsValue)
        {
            using (var connection = _connectionFactory.Invoke(configuration))
            {
                connection.ConnectionString = configuration.ConnectionString;

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

                _clearPoolAction(connection);
            }
        }

        public Task CommunicateAsync(IDbCommand carrier, Action<IDataReader> onDataAvailable,
            MeadowConfiguration configuration, bool returnsValue)
        {
            return Task.Run(() => { Communicate(carrier, onDataAvailable, configuration, returnsValue); });
        }
    }
}