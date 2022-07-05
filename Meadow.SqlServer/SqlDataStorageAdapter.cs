using System;
using System.Data;
using System.Data.SqlClient;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Sql;

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
        
    }
}