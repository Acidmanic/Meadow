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
        private readonly bool _quotesRoutineNames;

        public AdoStorageCommunication(Func<IDbCommand> commandFactory,
            Func<MeadowConfiguration, IDbConnection> connectionFactory, Action<IDbConnection> clearPoolAction,
            bool quotesRoutineNames)
        {
            _commandFactory = commandFactory;
            _connectionFactory = connectionFactory;
            _clearPoolAction = clearPoolAction;
            _quotesRoutineNames = quotesRoutineNames;
        }


        public IDbCommand CreateToStorageCarrier(MeadowRequest request, MeadowConfiguration configuration)
        {
            var carrier = _commandFactory.Invoke();

            if (request.Execution == RequestExecution.RequestTextIsNameOfRoutine)
            {
                carrier.CommandType = CommandType.StoredProcedure;
                carrier.CommandText = _quotesRoutineNames ? Quot(request.RequestText) : request.RequestText;
            }
            else
            {
                carrier.CommandType = CommandType.Text;
                carrier.CommandText = request.RequestText;
            }

            return carrier;
        }

        private string Quot(string value)
        {
            return $"\"{value}\"";
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