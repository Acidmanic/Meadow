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
    }
}