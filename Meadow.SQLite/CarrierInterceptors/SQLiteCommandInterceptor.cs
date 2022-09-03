using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Contracts;
using Meadow.Sql;

namespace Meadow.SQLite.CarrierInterceptors
{
    public class SQLiteCommandInterceptor:ICarrierInterceptor<IDbCommand,IDataReader>
    {
        public void InterceptBeforeCommunication(IDbCommand carrier, ObjectEvaluator evaluator)
        {
            var commandText = carrier.CommandText;
            
            var procedure = SqLiteProcedureManager.Instance.GetProcedureOrNull(commandText);
            
            if (procedure != null)
            {
                var standardData = new Record(evaluator.ToStandardFlatData()
                    // Select Only Direct-Leaves
                    .Where(dp =>
                    {
                        var node = evaluator.Map.NodeByAddress(dp.Identifier);

                        return node.IsLeaf && node.Parent == evaluator.RootNode;
                    }));

                List<DataPoint> data = new FieldAddressTranslatedStandardDataTranslator(
                        new RelationalRelationalIdentifierToStandardFieldMapper())
                    .TranslateToStorage(standardData, evaluator)
                    .ToList();
                
                var injectedCode = InjectValuesIntoCode(procedure, data);
            
                commandText = injectedCode;   
            }
            else
            {
                SqLiteProcedure p = SqLiteProcedure.Parse(commandText);
            
                if(p!=null)
                {
                    SqLiteProcedureManager.Instance.AddProcedure(p);
            
                    commandText = "";
                }
            }
            
            carrier.CommandText = commandText;
        }
        
        
        
        private string InjectValuesIntoCode(SqLiteProcedure procedure, List<DataPoint> data)
        {
            var code = procedure.Code;

            foreach (var parameterName in procedure.Parameters.Keys)
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
            var atlessName = parameterName.Substring(1, parameterName.Length - 1);
            
            var dp = data.SingleOrDefault(dataPoint => dataPoint.Identifier == atlessName);

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

        public void InterceptAfterCommunication(IDataReader carrier)
        {
            // Its OK.
        }
    }
}