using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.DataAccessCore
{
    public abstract class MeadowDataAccessCoreBase<TToStorageCarrier, TFromStorageCarrier> : IMeadowDataAccessCore
    {

        protected ILogger Logger { get; private set; }= NullLogger.Instance;
        
        protected virtual IDataOwnerNameProvider DataOwnerNameProvider { get; private set; }

        private List<ICarrierInterceptor<TToStorageCarrier, TFromStorageCarrier>> _carrierInterceptors;

        public MeadowDataAccessCoreBase()
        {
            _carrierInterceptors = new List<ICarrierInterceptor<TToStorageCarrier, TFromStorageCarrier>>();
        }

        protected abstract IStandardDataStorageAdapter<TToStorageCarrier, TFromStorageCarrier> DataStorageAdapter { get; set; }

        protected abstract IStorageCommunication<TToStorageCarrier, TFromStorageCarrier> StorageCommunication { get; set; }

        
        public  MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new()
        {
            return PerformRequestAsync(request, configuration).Result;
        }

        public virtual async Task<MeadowRequest<TIn, TOut>> PerformRequestAsync<TIn, TOut>(MeadowRequest<TIn, TOut> request, MeadowConfiguration configuration) where TOut : class, new()
        {
            request.InitializeBeforeExecution();


            var carrier = ProvideCarrier(request, configuration);


            void OnDataAvailable(TFromStorageCarrier reader)
            {
                InterceptFromStorage(reader);

                request.FromStorage = DataStorageAdapter.ReadFromStorage<TOut>(reader, request.FromStorageInclusion,request.FullTree);
            }
            
            try
            {
                await StorageCommunication.CommunicateAsync(carrier, OnDataAvailable, configuration, request.ReturnsValue);
            }
            catch (Exception e)
            {
                request.SetFailure(e);
            }

            return request;
        }

        public abstract void CreateDatabase(MeadowConfiguration configuration);
        public abstract Task CreateDatabaseAsync(MeadowConfiguration configuration);

        public abstract bool CreateDatabaseIfNotExists(MeadowConfiguration configuration);
        public abstract Task<bool> CreateDatabaseIfNotExistsAsync(MeadowConfiguration configuration);

        public abstract void DropDatabase(MeadowConfiguration configuration);
        public abstract Task DropDatabaseAsync(MeadowConfiguration configuration);

        public abstract bool DatabaseExists(MeadowConfiguration configuration);
        public abstract Task<bool> DatabaseExistsAsync(MeadowConfiguration configuration);

        public abstract List<string> EnumerateProcedures(MeadowConfiguration configuration);
        public abstract Task<List<string>> EnumerateProceduresAsync(MeadowConfiguration configuration);

        public abstract List<string> EnumerateTables(MeadowConfiguration configuration);
        public abstract Task<List<string>> EnumerateTablesAsync(MeadowConfiguration configuration);

        public abstract void CreateTable<TModel>(MeadowConfiguration configuration);
        public abstract Task CreateTableAsync<TModel>(MeadowConfiguration configuration);

        public abstract void CreateInsertProcedure<TModel>(MeadowConfiguration configuration);
        public abstract Task CreateInsertProcedureAsync<TModel>(MeadowConfiguration configuration);

        public abstract void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration);
        public abstract Task CreateLastInsertedProcedureAsync<TModel>(MeadowConfiguration configuration);
        
        public abstract void CreateReadAllProcedure<TModel>(MeadowConfiguration configuration);
        public abstract Task CreateReadAllProcedureAsync<TModel>(MeadowConfiguration configuration);

        public IMeadowDataAccessCore Initialize(MeadowConfiguration configuration,ILogger logger)
        {
            // That must be read from configurations later on
            DataOwnerNameProvider = new PluralDataOwnerNameProvider();

            Logger = logger;

            return InitializeDerivedClass(configuration);
        }

        public abstract IFilterQueryTranslator ProvideFilterQueryTranslator();

        protected virtual IMeadowDataAccessCore InitializeDerivedClass(MeadowConfiguration configuration)
        {
            return this;
        }

        protected virtual TToStorageCarrier ProvideCarrier<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new()
        {
            var carrier = StorageCommunication.CreateToStorageCarrier(request, configuration);

            var storage = request.ToStorage;

            if (storage is null)
            {
                InterceptToStorage(carrier, new ObjectEvaluator(typeof(TIn)));

                return carrier;
            }

            var evaluator = new ObjectEvaluator(storage);

            InterceptToStorage(carrier, evaluator);

            DataStorageAdapter.WriteToStorage(carrier, request.ToStorageInclusion, evaluator);

            return carrier;
        }


        protected List<TOut> PerformConfigurationRequest<TOut>(ConfigurationRequest<TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new()
        {
            return PerformConfigurationRequestAsync(request, configuration).Result;
        }
        
        protected async Task<List<TOut>> PerformConfigurationRequestAsync<TOut>(ConfigurationRequest<TOut> request,
            MeadowConfiguration configuration)
            where TOut : class, new()
        {
            try
            {
                var config = request.PreConfigure(configuration);

                var response = await PerformRequestAsync(request, config);

                if (response.Failed)
                {
                    Logger.LogError(response.FailureException,
                        "Meadow Configuration Exception:\n {Exception}",response.FailureException);
                }

                return response.FromStorage;
            }
            catch (Exception e)
            {
                Logger.LogError(e,"Meadow Configuration Exception:\n {Exception}",e);
            }

            return new List<TOut>();
        }


        protected void AddCarrierInterceptor(
            ICarrierInterceptor<TToStorageCarrier, TFromStorageCarrier> carrierInterceptor)
        {
            _carrierInterceptors.Add(carrierInterceptor);
        }

        private void InterceptToStorage(TToStorageCarrier carrier, ObjectEvaluator data)
        {
            _carrierInterceptors.ForEach(i => i.InterceptBeforeCommunication(carrier, data));
        }

        private void InterceptFromStorage(TFromStorageCarrier carrier)
        {
            _carrierInterceptors.ForEach(i => i.InterceptAfterCommunication(carrier));
        }
    }
}