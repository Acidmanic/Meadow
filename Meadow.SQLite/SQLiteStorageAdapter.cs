using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Requests;
using Meadow.Sql;

namespace Meadow.SQLite
{
    public class SqLiteStorageAdapter : SqlDataStorageAdapterBase
    {
        protected override void WriteAllToCommand(List<DataPoint> data, IDbCommand command)
        {
            var procedure = SqLiteInMemoryProcedures.Instance.GetProcedureOrNull(command.CommandText);

            if (procedure != null)
            {
                var injectedCode = InjectValuesIntoCode(procedure, data);

                command.CommandText = injectedCode;   
            }
        }

        private string InjectValuesIntoCode(SqLiteProcedure procedure, List<DataPoint> data)
        {
            var code = procedure.Code;

            foreach (var parameterName in procedure.ParameterNames)
            {
                var value = GetValueString(parameterName, data);

                if (value == null)
                {
                    throw new ArgumentException($"Procedure {procedure.Name} expects a value for " +
                                                $"parameter {parameterName} which is not provided.");
                }

                code = code.Replace(parameterName, value);
            }

            return code;
        }

        private string GetValueString(string parameterName, List<DataPoint> data)
        {
            var dp = data.SingleOrDefault(dataPoint => dataPoint.Identifier == parameterName);

            var value = dp?.Value;

            if (value == null)
            {
                return null;
            }

            if (value is string)
            {
                return $"'{value}'";
            }

            return value.ToString();
        }

        protected override void WriteIntoCommand(DataPoint dataPoint, IDbCommand command)
        {
            throw new NotImplementedException();
        }
    }
}