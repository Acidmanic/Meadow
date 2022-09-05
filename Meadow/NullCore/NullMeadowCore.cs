using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.DataAccessCore;
using Meadow.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.NullCore
{
    public class NullMeadowCore : IMeadowDataAccessCore
    {

        private readonly ILogger _logger  ;

        public NullMeadowCore(ILogger logger)
        {
            _logger = logger;
        }

        public NullMeadowCore():this(NullLogger.Instance)
        {
            
        }

        public MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration) where TOut : class, new()
        {
            _logger.LogError("No DataAccessCore has been introduced to meadow engine.");

            if (request.ReturnsValue)
            {
                request.FromStorage = new List<TOut>();
            }

            return request;
        }

        public void CreateDatabase(MeadowConfiguration configuration)
        {
        }

        public bool CreateDatabaseIfNotExists(MeadowConfiguration configuration)
        {
            return false;
        }

        public void DropDatabase(MeadowConfiguration configuration)
        {
        }

        public bool DatabaseExists(MeadowConfiguration configuration)
        {
            return false;
        }

        public List<string> EnumerateProcedures(MeadowConfiguration configuration)
        {
            return new List<string>();
        }

        public List<string> EnumerateTables(MeadowConfiguration configuration)
        {
            return new List<string>();
        }

        public void CreateTable<TModel>(MeadowConfiguration configuration)
        {
        }

        public void CreateInsertProcedure<TModel>(MeadowConfiguration configuration)
        {
        }

        public void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
        }

        public IMeadowDataAccessCore Initialize(MeadowConfiguration configuration)
        {
            return this;
        }


        public TModel ReadLastInsertedRecord<TModel>(MeadowConfiguration configuration) where TModel : class, new()
        {
            return null;
        }
    }
}