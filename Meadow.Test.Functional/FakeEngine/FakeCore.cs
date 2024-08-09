using System.Collections.Generic;
using System.Threading.Tasks;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore;

namespace Meadow.Test.Functional.FakeEngine
{
    public class FakeCore:MeadowDataAccessCoreBase<int,int>
    {
        protected override IStandardDataStorageAdapter<int, int> DataStorageAdapter { get; set; } = new FakeAdapter();
        protected override IStorageCommunication<int, int> StorageCommunication { get; set; } = new FakeCommunication();
        public override void CreateDatabase(MeadowConfiguration configuration)
        {
            
        }

        public override Task CreateDatabaseAsync(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public override bool CreateDatabaseIfNotExists(MeadowConfiguration configuration)
        {
            return false;
        }

        public override Task<bool> CreateDatabaseIfNotExistsAsync(MeadowConfiguration configuration)
        {
            return Task.Run(() => false);
        }

        public override void DropDatabase(MeadowConfiguration configuration)
        {
        }

        public override Task DropDatabaseAsync(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public override bool DatabaseExists(MeadowConfiguration configuration)
        {
            return false;
        }

        public override Task<bool> DatabaseExistsAsync(MeadowConfiguration configuration)
        {
            return Task.Run(() => false);
        }

        public override List<string> EnumerateProcedures(MeadowConfiguration configuration)
        {
            return new List<string>();
        }

        public override Task<List<string>> EnumerateProceduresAsync(MeadowConfiguration configuration)
        {
            return Task.Run(() => new List<string>());
        }

        public override List<string> EnumerateTables(MeadowConfiguration configuration)
        {
            return new List<string>();
        }

        public override Task<List<string>> EnumerateTablesAsync(MeadowConfiguration configuration)
        {
            return Task.Run(() => new List<string>());
        }

        public override void CreateTable<TModel>(MeadowConfiguration configuration)
        {
            
        }

        public override Task CreateTableAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public override void CreateInsertProcedure<TModel>(MeadowConfiguration configuration)
        {
            
        }

        public override Task CreateInsertProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public override void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
            
        }

        public override Task CreateLastInsertedProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public override void CreateReadAllProcedure<TModel>(MeadowConfiguration configuration)
        {
            
        }

        public override Task CreateReadAllProcedureAsync<TModel>(MeadowConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        public override ISqlExpressionTranslator ProvideFilterQueryTranslator(MeadowConfiguration configuration)
        {
            return ISqlExpressionTranslator.NullSqlExpressionTranslator.Instance;
        }
    }
}