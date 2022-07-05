using System.Data;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;
using Meadow.Sql;

namespace Meadow.DataAccessCore
{
    public abstract class MeadowDataAccessCoreBase<TToStorageCarrier, TFromStorageCarrier>:IMeadowDataAccessCore
    {
        public abstract IDataOwnerNameProvider DataOwnerNameProvider { get; }

        public MeadowDataAccessCoreBase()
        {
        }

        protected abstract IStandardDataStorageAdapter<TToStorageCarrier, TFromStorageCarrier> DataStorageAdapter { get; }

        protected abstract IStorageCommunication<TToStorageCarrier, TFromStorageCarrier> StorageCommunication { get; }

        public MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new()
        {
            request.InitializeBeforeExecution(DataOwnerNameProvider);


            var carrier = ProvideCarrier(request, configuration);


            void OnDataAvailable(TFromStorageCarrier reader)
            {
                request.FromStorage = DataStorageAdapter.ReadFromStorage<TOut>(reader, request.FromStorageMarks);
            }

            StorageCommunication.Communicate(carrier, OnDataAvailable, configuration, request.ReturnsValue);

            return request;
        }

        private TToStorageCarrier ProvideCarrier<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new()
        {
            var carrier = StorageCommunication.CreateToStorageCarrier(request, configuration);

            var storage = request.ToStorage;

            if (storage is null)
            {
                return carrier;
            }

            var evaluator = new ObjectEvaluator(storage);

            DataStorageAdapter.WriteToStorage(carrier, request.ToStorageMarks, evaluator);

            return carrier;
        }
    }
}