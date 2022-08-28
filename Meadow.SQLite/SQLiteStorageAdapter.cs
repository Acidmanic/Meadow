using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Requests;
using Meadow.Sql;

namespace Meadow.SQLite
{
    public class SqLiteStorageAdapter : SqlDataStorageAdapterBase
    {

        protected override void WriteIntoCommand(DataPoint dataPoint, IDbCommand command)
        {
            // We dont do that here!
            //throw new NotImplementedException();
        }

        public SqLiteStorageAdapter(char fieldNameDelimiter, IDataOwnerNameProvider dataOwnerNameProvider) : base(fieldNameDelimiter, dataOwnerNameProvider)
        {
        }
    }
}