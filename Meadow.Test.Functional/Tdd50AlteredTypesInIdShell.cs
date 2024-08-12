// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using Acidmanic.Utilities.Reflection;
// using Acidmanic.Utilities.Reflection.Attributes;
// using Acidmanic.Utilities.Reflection.Dynamics;
// using Acidmanic.Utilities.Reflection.Extensions;
// using Acidmanic.Utilities.Reflection.ObjectTree;
// using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
// using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
// using Meadow.Contracts;
// using Meadow.Extensions;
// using Meadow.RelationalStandardMapping;
// using Meadow.Test.Functional.TDDAbstractions;
// using PropertyWrapper = Meadow.Test.Functional.Utility.PropertyWrapper;
//
// namespace Meadow.Test.Functional;
//
// public class Tdd50AlteredTypesInIdShell:IFunctionalTest
// {
//
// [AlteredType(typeof(string))]
//     // public struct Id
//     // {
//     //     public Id(string value)
//     //     {
//     //         Value = value;
//     //     }
//     //
//     //     public string Value { get; set; }
//     //
//     //
//     //     public static implicit operator string(Id value) => value.Value;
//     //     public static implicit operator Id(string value) => new Id(value);
//     // }
//
//     private class Model
//     {
//         [TreatAsLeaf]
//         [UniqueMember]
//         public Id Id { get; set; }
//         
//         public string Name { get; set; }
//     }
//     
//     
//     
//     
//     public void Main()
//     {
//         var model = new Model() { Id = "69aebcea-3c90-11ef-a44a-ffbea68c3cb4", Name = "Mani" };
//
//         var idField = TypeIdentity.FindIdentityLeaf(typeof(Model));
//         
//         object shell = new ModelBuilder("IdShellTwo").AddProperty(idField.Name, idField.Type).BuildObject();
//         
//         PropertyWrapper.Create(idField.Name, idField.Type, shell).Value = (object) model.Id;
//
//         var mapDictionary = GetDirectMap(shell.GetType());
//
//         var evaluator = new ObjectEvaluator(shell);
//         
//         var standardData = ReadParameters(evaluator);
//
//         var mapper = new FlatRelationalToStandardMapper(new PluralDataOwnerNameProvider());
//         
//         standardData = standardData.StandardToRelational(mapper, evaluator.RootNode.Type, false);
//         
//         var value = GetValueString("@Id", standardData);
//         
//         var expansionValue = GetValueString("@Id", standardData,true);
//     }
//     
//     
//     public Dictionary<string, FieldKey> GetDirectMap(Type type)
//     {
//         Dictionary<string, FieldKey> directMap = new Dictionary<string, FieldKey>();
//         ObjectEvaluator objectEvaluator = new ObjectEvaluator(type);
//         foreach (AccessNode directLeaf in objectEvaluator.RootNode.GetDirectLeaves())
//         {
//             FieldKey fieldKey = objectEvaluator.Map.FieldKeyByNode(directLeaf);
//             directMap.Add(directLeaf.Name, fieldKey);
//         }
//         return directMap;
//     }
//     
//     private Record ReadParameters(ObjectEvaluator evaluator)
//     {
//         var record = new Record();
//             
//         foreach (var child in evaluator.RootNode.GetChildren())
//         {
//             if (child.IsLeaf || 
//                 child.PropertyAttributes.Any(p => p is TreatAsLeafAttribute) || 
//                 child.Type.GetCustomAttributes().Any(a => a is AlteredTypeAttribute))
//             {
//                 var address = evaluator.Map.AddressByNode(child);
//                 var key = evaluator.Map.FieldKeyByNode(child);
//                 var value = evaluator.Read(key, true);
//                 record.Add(address,value);                    
//             }
//         }
//
//         return record;
//     }
//     
//     private string GetValueString(string parameterName, List<DataPoint> data,bool expansion = false)
//     {
//         var atLessName = parameterName.Substring(1, parameterName.Length - 1);
//
//         var dp = data.SingleOrDefault(dataPoint => dataPoint.Identifier == atLessName);
//
//         var value = dp?.Value;
//
//         if (expansion)
//         {
//             return value?.ToString() ?? "";
//         }
//             
//         if (value == null)
//         {
//             return "";
//         }
//
//         var type = value.GetType();
//
//         var altered = type.GetCustomAttribute<AlteredTypeAttribute>();
//
//         if (altered != null)
//         {
//             type = altered.AlternativeType;
//         }
//
//         value = value.CastTo(type);
//                 
//         if (value is string stringValue)
//         {
//             stringValue = EscapeSingleQuotes(stringValue);
//                 
//             return $"'{stringValue}'";
//         }
//
//         return value.ToString();
//     }
//     
//     private string EscapeSingleQuotes(string value)
//     {
//         var key = $"<{Guid.NewGuid():N}>";
//         // Just In Case!
//         value = value.Replace("'", key);
//         value = value.Replace(key, "''");
//
//         return value;
//     }
// }