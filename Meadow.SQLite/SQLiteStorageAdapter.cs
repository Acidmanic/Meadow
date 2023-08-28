using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Configuration;
using Meadow.Requests;
using Meadow.Sql;
using Microsoft.Extensions.Logging;

namespace Meadow.SQLite
{
    public class SqLiteStorageAdapter : SqlDataStorageAdapterBase
    {
        protected override void WriteIntoCommand(DataPoint dataPoint, IDbCommand command)
        {
            // We dont do that here!
            //throw new NotImplementedException();
        }

        public SqLiteStorageAdapter(MeadowConfiguration configuration, ILogger logger) : base(configuration, logger)
        {
        }
    }
}