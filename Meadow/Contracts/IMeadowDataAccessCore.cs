using System.Collections.Generic;
using System.Threading.Tasks;
using Meadow.Configuration;
using Meadow.Requests;
using Microsoft.Extensions.Logging;

namespace Meadow.Contracts
{
    public interface IMeadowDataAccessCore
    {
        MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class;
        
        Task<MeadowRequest<TIn, TOut>> PerformRequestAsync<TIn, TOut>(
            MeadowRequest<TIn, TOut> request,
            MeadowConfiguration configuration)
            where TOut : class;


        void CreateDatabase(MeadowConfiguration configuration);
        Task CreateDatabaseAsync(MeadowConfiguration configuration);

        /// <summary>
        /// This method will create database only it is not already created.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>True if it created a new database</returns>
        bool CreateDatabaseIfNotExists(MeadowConfiguration configuration);
        
        Task<bool> CreateDatabaseIfNotExistsAsync(MeadowConfiguration configuration);

        void DropDatabase(MeadowConfiguration configuration);
        
        Task DropDatabaseAsync(MeadowConfiguration configuration);

        bool DatabaseExists(MeadowConfiguration configuration);
        
        Task<bool> DatabaseExistsAsync(MeadowConfiguration configuration);

        List<string> EnumerateProcedures(MeadowConfiguration configuration);
        
        Task<List<string>> EnumerateProceduresAsync(MeadowConfiguration configuration);

        List<string> EnumerateTables(MeadowConfiguration configuration);
        
        Task<List<string>> EnumerateTablesAsync(MeadowConfiguration configuration);

        void CreateTable<TModel>(MeadowConfiguration configuration);
        
        Task CreateTableAsync<TModel>(MeadowConfiguration configuration);

        void CreateInsertProcedure<TModel>(MeadowConfiguration configuration);
        
        Task CreateInsertProcedureAsync<TModel>(MeadowConfiguration configuration);

        void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration);
        
        Task CreateLastInsertedProcedureAsync<TModel>(MeadowConfiguration configuration);
        
        void CreateReadAllProcedure<TModel>(MeadowConfiguration configuration);
        
        Task CreateReadAllProcedureAsync<TModel>(MeadowConfiguration configuration);

        IMeadowDataAccessCore Initialize(MeadowConfiguration configuration,ILogger logger);
    }
}