using System.Data;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Contracts;
using Meadow.DataAccessCore;

namespace Meadow.Sql
{
    public class SqlDataAccessCore:MeadowDataAccessCoreBase<IDbCommand,IDataReader>
    {
        public override IDataOwnerNameProvider DataOwnerNameProvider { get; } = new PluralDataOwnerNameProvider();
        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; } = new SqlDataStorageAdapter();
        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; } = new SqlCommunication();
        
        
    }
}