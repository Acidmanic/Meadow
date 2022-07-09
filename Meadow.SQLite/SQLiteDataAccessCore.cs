using System.Collections.Generic;
using System.Data;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.Requests;

namespace Meadow.SQLite
{
    public class SQLiteDataAccessCore:MeadowDataAccessCoreBase<IDbCommand,IDataReader>
    {
        public override IDataOwnerNameProvider DataOwnerNameProvider { get; } = new PluralDataOwnerNameProvider();
        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; } = new SqLiteStorageAdapter();
        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; } = new SQLiteStorageCommunication();
        public override void CreateDatabase(MeadowConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }

        public override void CreateDatabaseIfNotExists(MeadowConfiguration configuration)
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