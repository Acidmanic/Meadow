using System;
using System.Data;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Sql;
using Microsoft.Extensions.Logging;

namespace Meadow.DataAccessCore.AdoCoreBase
{
    internal class AdoStorageAdapterBase:SqlDataStorageAdapterBase
    {

        private readonly Action<DataPoint, IDbCommand> _writeAction;
        
        public AdoStorageAdapterBase(char fieldNameDelimiter, IDataOwnerNameProvider dataOwnerNameProvider, ILogger logger, Action<DataPoint, IDbCommand> writeAction) : base(fieldNameDelimiter, dataOwnerNameProvider, logger)
        {
            _writeAction = writeAction;
        }

        protected override void WriteIntoCommand(DataPoint dataPoint, IDbCommand command)
        {
            _writeAction(dataPoint, command);
        }
    }
}