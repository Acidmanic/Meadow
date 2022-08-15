using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore;

namespace Meadow.Test.Functional.FakeEngine
{
    public class FakeCore:MeadowDataAccessCoreBase<int,int>
    {
        public override IDataOwnerNameProvider DataOwnerNameProvider { get; } = new PluralDataOwnerNameProvider();
        protected override IStandardDataStorageAdapter<int, int> DataStorageAdapter { get; } = new FakeAdapter();
        protected override IStorageCommunication<int, int> StorageCommunication { get; } = new FakeCommunication();
        public override void CreateDatabase(MeadowConfiguration configuration)
        {
            
        }

        public override bool CreateDatabaseIfNotExists(MeadowConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }

        public override void DropDatabase(MeadowConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }

        public override bool DatabaseExists(MeadowConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> EnumerateProcedures(MeadowConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> EnumerateTables(MeadowConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }

        public override void CreateTable<TModel>(MeadowConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }

        public override void CreateInsertProcedure<TModel>(MeadowConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }

        public override void CreateLastInsertedProcedure<TModel>(MeadowConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }
    }
}