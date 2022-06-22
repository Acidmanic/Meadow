using System.Data;
using System.Data.SqlClient;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;
using Meadow.Sql;

namespace Meadow
{
    internal class MeadowDataAccessCore
    {

        private readonly IDataOwnerNameProvider _dataOwnerNameProvider;

        public MeadowDataAccessCore(IDataOwnerNameProvider dataOwnerNameProvider)
        {
            _dataOwnerNameProvider = dataOwnerNameProvider;
        }

        public enum RequestExecutionType
        {
            Procedure = CommandType.StoredProcedure,
            Script = CommandType.Text
        }

        

        protected IStandardDataStorageAdapter<SqlCommand, IDataReader> DataStorageAdapter { get; } =
            new SqlDataStorageAdapter();

        public MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration,
            RequestExecutionType executionType)
            where TOut : class, new()
        {
            request.InitializeBeforeExecution(_dataOwnerNameProvider);

            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                var command = CreateCommand(request, configuration, (CommandType) executionType);

                command.Connection = connection;

                connection.Open();

                if (request.ReturnsValue)
                {
                    var dataReader = command.ExecuteReader(CommandBehavior.Default);

                    var fromStorage = DataStorageAdapter.ReadFromStorage<TOut>(
                        dataReader, request.FromStorageMarks);

                    connection.Close();

                    request.FromStorage = fromStorage;
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

            var evaluator = new ObjectEvaluator(storage);

            DataStorageAdapter.WriteToStorage(command, request.ToStorageMarks, evaluator);

            return command;
        }
    }
}