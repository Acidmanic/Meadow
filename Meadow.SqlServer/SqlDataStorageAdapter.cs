using System;
using System.Data;
using System.Data.SqlClient;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Configuration;
using Meadow.Sql;
using Microsoft.Extensions.Logging;

namespace Meadow.SqlServer
{
    public class SqlDataStorageAdapter : SqlDataStorageAdapterBase
    {
        protected override void WriteIntoCommand(DataPoint dataPoint, IDbCommand command)
        {
            var parameter = new SqlParameter("@" + dataPoint.Identifier, dataPoint.Value ?? DBNull.Value)
            {
                Direction = ParameterDirection.Input
            };

            command.Parameters.Add(parameter);
        }

        public SqlDataStorageAdapter(MeadowConfiguration configuration, ILogger logger) : base(configuration, logger)
        {
        }
    }
}