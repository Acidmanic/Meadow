using System;
using System.Data;
using System.Data.SqlClient;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Sql;
using MySql.Data.MySqlClient;

namespace Meadow.MySql
{
    public class MySqlStorageAdapter:SqlDataStorageAdapterBase
    {
        protected override void WriteIntoCommand(DataPoint dataPoint, IDbCommand command)
        {
            var parameter = new MySqlParameter(dataPoint.Identifier, dataPoint.Value ?? DBNull.Value)
            {
                Direction = ParameterDirection.Input
            };

            command.Parameters.Add(parameter);
        }
    }
}