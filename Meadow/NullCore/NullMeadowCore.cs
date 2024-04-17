using System.Collections.Generic;
using System.Threading.Tasks;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.NullCore
{
    public class NullMeadowCore : IMeadowDataAccessCore
    {
        private ILogger _logger;

        public NullMeadowCore(ILogger logger)
        {
            _logger = logger;
        }

        public NullMeadowCore() : this(NullLogger.Instance)
        {
        }

        public MeadowRequest<TOut> PerformRequest<TOut>(MeadowRequest<TOut> request,
            MeadowConfiguration configuration)
        {
            _logger.LogError("No DataAccessCore has been introduced to meadow engine.");

            if (request.ReturnsValue)
            {
                request.FromStorage.Clear();
            }

            return request;
        }

        public Task<MeadowRequest<TOut>> PerformRequestAsync<TOut>(MeadowRequest<TOut> request,
            MeadowConfiguration configuration) 
        {
            return Task.Run(() => PerformRequest(request, configuration));
        }

        public void CreateDatabase(MeadowConfiguration configuration)
        {
        }

        public Task CreateDatabaseAsync(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public bool CreateDatabaseIfNotExists(MeadowConfiguration configuration)
        {
            return false;
        }

        public Task<bool> CreateDatabaseIfNotExistsAsync(MeadowConfiguration configuration)
        {
            return Task.Run(() => false);
        }

        public void DropDatabase(MeadowConfiguration configuration)
        {
        }

        public Task DropDatabaseAsync(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public bool DatabaseExists(MeadowConfiguration configuration)
        {
            return false;
        }

        public Task<bool> DatabaseExistsAsync(MeadowConfiguration configuration)
        {
            return Task.Run(() => false);
        }

        public List<string> EnumerateProcedures(MeadowConfiguration configuration)
        {
            return new List<string>();
        }

        public Task<List<string>> EnumerateProceduresAsync(MeadowConfiguration configuration)
        {
            return Task.Run(() => new List<string>());
        }

        public List<string> EnumerateTables(MeadowConfiguration configuration)
        {
            return new List<string>();
        }

        public Task<List<string>> EnumerateTablesAsync(MeadowConfiguration configuration)
        {
            return Task.Run(() => new List<string>());
        }

        public void CreateTable<TModel>(MeadowConfiguration configuration)
        {
        }

        public Task CreateTableAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public void CreateInsertProcedure<TModel>(MeadowConfiguration configuration)
        {
        }

        public Task CreateInsertProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
        }

        public Task CreateLastInsertedProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public void CreateReadAllProcedure<TModel>(MeadowConfiguration configuration)
        {
        }

        public Task CreateReadAllProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public IMeadowDataAccessCore Initialize(MeadowConfiguration configuration, ILogger logger)
        {
            _logger = logger;

            return this;
        }

        public ISqlFilteringTranslator ProvideFilterQueryTranslator()
        {
            return ISqlFilteringTranslator.Null;
        }


        public TModel ReadLastInsertedRecord<TModel>(MeadowConfiguration configuration) where TModel : class, new()
        {
            return null;
        }
    }
}