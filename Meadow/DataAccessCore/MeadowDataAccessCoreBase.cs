using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;
using Meadow.Requests.Configuration.Abstractions;
using Meadow.Requests.Context;
using Meadow.Scaffolding.Translators;
using Meadow.Sql;
using Meadow.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.DataAccessCore
{
    public abstract class MeadowDataAccessCoreBase<TToStorageCarrier, TFromStorageCarrier> : IMeadowDataAccessCore
    {
        protected ILogger Logger { get; private set; } = NullLogger.Instance;

        protected virtual IDataOwnerNameProvider DataOwnerNameProvider { get; private set; }

        private readonly List<ICarrierInterceptor<TToStorageCarrier, TFromStorageCarrier>> _carrierInterceptors;

        public MeadowDataAccessCoreBase()
        {
            _carrierInterceptors = new List<ICarrierInterceptor<TToStorageCarrier, TFromStorageCarrier>>();
        }

        protected abstract IStandardDataStorageAdapter<TToStorageCarrier, TFromStorageCarrier> DataStorageAdapter
        {
            get;
            set;
        }

        protected abstract IStorageCommunication<TToStorageCarrier, TFromStorageCarrier> StorageCommunication
        {
            get;
            set;
        }


        public MeadowRequest<TOut> PerformRequest<TOut>(
            MeadowRequest<TOut> request,
            MeadowConfiguration configuration)
        {
            return PerformRequestAsync(request, configuration).Result;
        }
        
        public MeadowRequest PerformRequest(
            MeadowRequest request,
            MeadowConfiguration configuration)
        {
            return PerformRequestAsync(request, configuration).Result;
        }

        public virtual async Task<MeadowRequest<TOut>> PerformRequestAsync<TOut>(
            MeadowRequest<TOut> request, MeadowConfiguration configuration)
        {

            var languageTranslator = ProvideSqlLanguageTranslator();

            var context = new MeadowExecutionContext(Logger,
                languageTranslator,
                new SqlFilteringTranslator(Logger,configuration,languageTranslator),
                configuration);
            
            request.StartExecution(context);
            
            var carrier = ProvideCarrier(request, configuration);

            void OnDataAvailable(TFromStorageCarrier reader)
            {
                InterceptFromStorage(reader, configuration);

                request.FromStorage.Clear();

                request.FromStorage.AddRange(DataStorageAdapter.ReadFromStorage<TOut>(reader));
            }

            try
            {
                await StorageCommunication.CommunicateAsync(carrier, OnDataAvailable, configuration,
                    request.ReturnsValue);
            }
            catch (Exception e)
            {
                request.SetFailure(e);
            }
            
            request.EndExecution();

            return request;
        }
        
        public virtual async Task<MeadowRequest> PerformRequestAsync(
            MeadowRequest request, MeadowConfiguration configuration)
        {

            var languageTranslator = ProvideSqlLanguageTranslator();

            var context = new MeadowExecutionContext(Logger,
                languageTranslator,
                new SqlFilteringTranslator(Logger,configuration,languageTranslator),
                configuration);
            
            request.StartExecution(context);
            
            var carrier = ProvideCarrier(request, configuration);

            void OnDataAvailable(TFromStorageCarrier reader)
            {
                InterceptFromStorage(reader, configuration);
                
            }

            try
            {
                await StorageCommunication.CommunicateAsync(carrier, OnDataAvailable, configuration,
                    request.ReturnsValue);
            }
            catch (Exception e)
            {
                request.SetFailure(e);
            }
            
            request.EndExecution();

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

        public IMeadowDataAccessCore Initialize(MeadowConfiguration configuration, ILogger logger)
        {
            DataOwnerNameProvider = configuration.TableNameProvider;

            Logger = logger;

            return InitializeDerivedClass(configuration);
        }

        public abstract ISqlLanguageTranslator ProvideSqlLanguageTranslator();

        protected virtual IMeadowDataAccessCore InitializeDerivedClass(MeadowConfiguration configuration)
        {
            return this;
        }

        protected virtual TToStorageCarrier ProvideCarrier(
            MeadowRequest request,
            MeadowConfiguration configuration)
        {
            var carrier = StorageCommunication.CreateToStorageCarrier(request, configuration);

            var storage = request.ToStorage;

            if (storage.Count == 0)
            {
                return carrier;
            }

            var toStorageData = request.ToStorage.Select(d => new ObjectEvaluator(d)).ToList();

            InterceptToStorage(carrier, toStorageData, configuration);

            DataStorageAdapter.WriteToStorage(carrier, request.InputInclusions, toStorageData);

            return carrier;
        }


        protected List<TOut> PerformConfigurationRequest<TOut>(ConfigurationRequest<TOut> request,
            MeadowConfiguration configuration)
        {
            return PerformConfigurationRequestAsync(request, configuration).Result;
        }
        
        protected void PerformConfigurationRequest(ConfigurationRequest request,
            MeadowConfiguration configuration)
        {
            PerformConfigurationRequestAsync(request, configuration).Wait();
        }

        protected async Task<List<TOut>> PerformConfigurationRequestAsync<TOut>(ConfigurationRequest<TOut> request,
            MeadowConfiguration configuration)
        {
            try
            {
                var parsedConnectionString = new ConnectionStringParser().Parse(configuration.ConnectionString);

                var config = request.AlterConfiguration(configuration, parsedConnectionString);

                var response = await PerformRequestAsync(request, config);

                if (response.Failed)
                {
                    Logger.LogError(response.FailureException,
                        "Meadow Configuration Exception:\n {Exception}", response.FailureException);
                }

                return response.FromStorage;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Meadow Configuration Exception:\n {Exception}", e);
            }

            return new List<TOut>();
        }
        
        protected async Task PerformConfigurationRequestAsync(ConfigurationRequest request,
            MeadowConfiguration configuration)
        {
            try
            {
                var parsedConnectionString = new ConnectionStringParser().Parse(configuration.ConnectionString);

                var config = request.AlterConfiguration(configuration, parsedConnectionString);

                var response = await PerformRequestAsync(request, config);

                if (response.Failed)
                {
                    Logger.LogError(response.FailureException,
                        "Meadow Configuration Exception:\n {Exception}", response.FailureException);
                }

                return;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Meadow Configuration Exception:\n {Exception}", e);
            }

        }


        protected void AddCarrierInterceptor(
            ICarrierInterceptor<TToStorageCarrier, TFromStorageCarrier> carrierInterceptor)
        {
            _carrierInterceptors.Add(carrierInterceptor);
        }

        private void InterceptToStorage(TToStorageCarrier carrier, List<ObjectEvaluator> data,
            MeadowConfiguration configuration)
        {
            _carrierInterceptors.ForEach(i => i.InterceptBeforeCommunication(carrier, data, configuration));
        }

        private void InterceptFromStorage(TFromStorageCarrier carrier, MeadowConfiguration configuration)
        {
            _carrierInterceptors.ForEach(i => i.InterceptAfterCommunication(carrier, configuration));
        }
    }
}