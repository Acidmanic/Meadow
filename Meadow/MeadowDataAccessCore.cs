using System;
using System.Data;
using System.Data.SqlClient;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.Requests;
using Meadow.Sql;

namespace Meadow
{
    internal class MeadowDataAccessCore:MeadowDataAccessCoreBase<IDbCommand,IDataReader>
    {
        public override IDataOwnerNameProvider DataOwnerNameProvider { get; } = new PluralDataOwnerNameProvider();
        protected override IStandardDataStorageAdapter<IDbCommand, IDataReader> DataStorageAdapter { get; } = new SqlDataStorageAdapter();
        protected override IStorageCommunication<IDbCommand, IDataReader> StorageCommunication { get; } = new SqlCommunication();
        
        
    }
}