using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
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

        private List<ICarrierInterceptor<TToStorageCarrier, TFromStorageCarrier>> _carrierInterceptors;
        
        public MeadowDataAccessCoreBase()
        {
            _carrierInterceptors = new List<ICarrierInterceptor<TToStorageCarrier, TFromStorageCarrier>>();
        }

        protected abstract IStandardDataStorageAdapter<TToStorageCarrier, TFromStorageCarrier> DataStorageAdapter { get; }

        protected abstract IStorageCommunication<TToStorageCarrier, TFromStorageCarrier> StorageCommunication { get; }

        
        
        public virtual MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new()
        {
            request.InitializeBeforeExecution(DataOwnerNameProvider);


            var carrier = ProvideCarrier(request, configuration);


            void OnDataAvailable(TFromStorageCarrier reader)
            {
                InterceptFromStorage(reader);
                
                request.FromStorage = DataStorageAdapter.ReadFromStorage<TOut>(reader, request.FromStorageMarks);
            }

            StorageCommunication.Communicate(carrier, OnDataAvailable, configuration, request.ReturnsValue);

            return request;
        }

        public abstract void CreateDatabase(MeadowConfiguration configuration);
        public abstract void CreateDatabaseIfNotExists(MeadowConfiguration configuration);
        public abstract void DropDatabase(MeadowConfiguration configuration);
        public abstract bool DatabaseExists(MeadowConfiguration configuration);
        public abstract List<string> EnumerateProcedures(MeadowConfiguration configuration);
        public abstract List<string> EnumerateTables(MeadowConfiguration configuration);
        public abstract void CreateTable<TModel>(MeadowConfiguration configuration);
        public abstract void CreateInsertProcedure<TModel>(MeadowConfiguration configuration);
        public abstract void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration);


        protected virtual TToStorageCarrier ProvideCarrier<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new()
        {
            var carrier = StorageCommunication.CreateToStorageCarrier(request, configuration);

            var storage = request.ToStorage;

            if (storage is null)
            {
                InterceptToStorage(carrier,new ObjectEvaluator(typeof(TIn)));
                
                return carrier;
            }

            var evaluator = new ObjectEvaluator(storage);

            InterceptToStorage(carrier,evaluator);
            
            DataStorageAdapter.WriteToStorage(carrier, request.ToStorageMarks, evaluator);
            
            return carrier;
        }

        protected void AddCarrierInterceptor(
            ICarrierInterceptor<TToStorageCarrier, TFromStorageCarrier> carrierInterceptor)
        {
            _carrierInterceptors.Add(carrierInterceptor);
        }

        private void InterceptToStorage(TToStorageCarrier carrier,ObjectEvaluator data)
        {
            _carrierInterceptors.ForEach(i => i.InterceptBeforeCommunication(carrier,data));
        }
        
        private void InterceptFromStorage(TFromStorageCarrier carrier)
        {
            _carrierInterceptors.ForEach(i => i.InterceptAfterCommunication(carrier));
        }
    }
}