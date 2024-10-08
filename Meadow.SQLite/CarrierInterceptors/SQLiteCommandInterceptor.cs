using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;
using Meadow.Sql;
using Meadow.SQLite.Exceptions;
using Meadow.SQLite.Extensions;
using Meadow.SQLite.ProcedureProcessing;

namespace Meadow.SQLite.CarrierInterceptors
{
    public class SqLiteCommandInterceptor : ICarrierInterceptor<IDbCommand, IDataReader>
    {
      
        public void InterceptBeforeCommunication(IDbCommand carrier, ObjectEvaluator evaluator,
            MeadowConfiguration configuration)
        {
            var commandText = carrier.CommandText;

            var manager = configuration.GetSqLiteProcedureManager();

            var procedure = manager.GetProcedureOrNull(commandText);

            if (procedure != null)
            {
                var standardData = ReadParameters(evaluator);

                var mapper = configuration.GetRelationalStandardMapper();
                
                standardData = standardData.StandardToRelational(mapper, evaluator.RootNode.Type, false);

                var injectedCode = InjectValuesIntoCode(procedure, standardData);

                commandText = injectedCode;
            }
            else
            {
                SqLiteProcedure p = SqLiteProcedure.Parse(commandText);

                if (p != null)
                {
                    manager.PerformProcedureCreation(p);

                    commandText = "";
                }
            }

            carrier.CommandText = commandText;
        }


        private Record ReadParameters(ObjectEvaluator evaluator)
        {
            var record = new Record();
            
            foreach (var child in evaluator.RootNode.GetChildren())
            {
                if (child.IsLeaf || 
                    child.PropertyAttributes.Any(p => p is TreatAsLeafAttribute) || 
                    child.Type.GetCustomAttributes().Any(a => a is AlteredTypeAttribute))
                {
                    var address = evaluator.Map.AddressByNode(child);
                    var key = evaluator.Map.FieldKeyByNode(child);
                    var value = evaluator.Read(key, true);
                    record.Add(address,value);                    
                }
            }

            return record;
        }

        private string InjectValuesIntoCode(SqLiteProcedure procedure, List<DataPoint> data)
        {
            var code = procedure.Code;

            foreach (var parameter in procedure.ParameterTypesByParameterName)
            {
                var parameterName = parameter.Key;
                var parameterType = parameter.Value;
                
                var value = GetValueString(parameterName, parameterType, data);
                var expansionValue = GetValueString(parameterName, parameterType,data,true);

                if (value == null)
                {
                    throw new ArgumentException($"Procedure {procedure.Name} expects a value for " +
                                                $"parameter {parameterName} which is not provided.");
                }

                code = code.Replace("&"+parameterName, expansionValue);
                code = code.Replace(parameterName, value);
            }

            return code;
        }

        private string GetValueString(string parameterName, string parameterType, List<DataPoint> data, bool expansion = false)
        {
            var atLessName = parameterName.Substring(1, parameterName.Length - 1);

            var dp = data.SingleOrDefault(dataPoint => dataPoint.Identifier == atLessName);

            var value = dp?.Value;

            if (expansion)
            {
                return value?.ToString() ?? "";
            }
            
            if (value == null)
            {
                return "";
            }

            var type = value.GetType();

            var altered = type.GetCustomAttribute<AlteredTypeAttribute>();

            if (altered != null)
            {
                type = altered.AlternativeType;
            }

            value = value.CastTo(type);
                
            if (parameterType.ToUpper() =="TEXT")
            {
                var stringValue = EscapeSingleQuotes($"{value}");
                
                return $"'{stringValue}'";
            }

            return value.ToString();
        }

        private string EscapeSingleQuotes(string value)
        {
            var key = $"<{Guid.NewGuid():N}>";
            // Just In Case!
            value = value.Replace("'", key);
            value = value.Replace(key, "''");

            return value;
        }

        public void InterceptAfterCommunication(IDataReader carrier, MeadowConfiguration configuration)
        {
            // Its OK.
        }
    }
}