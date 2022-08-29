// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Acidmanic.Utilities.Reflection.ObjectTree;
// using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
// using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
// using Meadow.Contracts;
//
// namespace Meadow.Sql
// {
//     public class SqlStandardDataTranslator:IStandardDataTranslator
//     {
//         public IDataOwnerNameProvider TableNameProvider { get; } = new PluralDataOwnerNameProvider();
//         
//         
//         public List<DataPoint> TranslateToStorage(Record standardData, ObjectEvaluator evaluator)
//         {
//             var keyIndexedData = IndexByKey(standardData, evaluator);
//
//             var idCounts = CountFieldNames(keyIndexedData.Keys);
//
//             List<DataPoint> translated = new List<DataPoint>();
//
//             foreach (var dataItem in keyIndexedData)
//             {
//                 var key = dataItem.Key;
//
//                 var value = dataItem.Value;
//
//                 var id = key.Last().Name;
//
//                 var fieldId = id;
//
//                 if (idCounts[id] > 1)
//                 {
//                     var tableName = GetTableNameOfFieldKey(key, evaluator);
//
//                     id = tableName + "." + id;
//                 }
//
//                 var dataPoint = new DataPoint
//                 {
//                     Identifier = fieldId,
//                     Value = value
//                 };
//                 translated.Add(dataPoint);
//             }
//
//             return translated;
//         }
//
//         private string GetTableNameOfFieldKey(FieldKey key, ObjectEvaluator evaluator)
//         {
//             var tableKey = key.UpLevel();
//
//             return GetTableNameOfTableKey(tableKey, evaluator);
//         }
//
//         private string GetTableNameOfTableKey(FieldKey tableKey, ObjectEvaluator evaluator)
//         {
//             var tableType = evaluator.Map.NodeByKey(tableKey).Type;
//
//             var tableName = TableNameProvider.GetNameForOwnerType(tableType);
//
//             return tableName;
//         }
//
//         public List<Record> TranslateFromStorage(List<Record> storageData, Type targetType)
//         {
//             var evaluator = new ObjectEvaluator(targetType);
//
//             // map to standard
//
//             List<string> availableFieldIds = EnumerateFieldIds(storageData);
//
//             var mappedFieldIds = MapFieldIdsToStandardKeys(availableFieldIds, evaluator);
//
//             var datapointComparator = new StandardDataPointComparator(mappedFieldIds);
//
//             var recordAccumulator = new SqlRecordAccumulator(evaluator.Map);
//
//             foreach (var record in storageData)
//             {
//                 record.Sort(datapointComparator);
//
//                 foreach (var datapoint in record)
//                 {
//                     var value = datapoint.Value;
//
//                     if (mappedFieldIds.ContainsKey(datapoint.Identifier))
//                     {
//                         var profile = mappedFieldIds[datapoint.Identifier];
//
//                         recordAccumulator.Pass(value, profile);    
//                     }
//                     else
//                     {
//                         //TODO: log if you want
//                         Console.WriteLine($"Could not find object-node for {datapoint.Identifier}");
//                     }
//
//                     
//                 }
//             }
//
//             return recordAccumulator.StandardRecords;
//         }
//
//
//         private Dictionary<string, FieldProfile> MapFieldIdsToStandardKeys(List<string> availableFieldIds,
//             ObjectEvaluator evaluator)
//         {
//             var map = new Dictionary<string, FieldProfile>();
//
//             foreach (var fieldId in availableFieldIds)
//             {
//                 var fieldKey = FieldKey.Parse(fieldId);
//
//                 var id = fieldKey.TerminalSegment().Name;
//
//                 var candidates = evaluator.Map.Keys
//                     .Where(k => k.TerminalSegment().Name == id)
//                     .ToList();
//
//                 if (candidates.Count == 0)
//                 {
//                     //TODO: Log missing field for received data.
//                 }
//                 else if (candidates.Count() == 1)
//                 {
//                     map.Add(fieldId, new FieldProfile(candidates[0], evaluator.Map.NodeByKey(candidates[0])));
//                 }
//                 else if (candidates.Count > 1)
//                 {
//                     List<FieldKey> narrowedDown;
//
//
//                     if (fieldKey.Count < 2)
//                     {
//                         narrowedDown = candidates.Where(key => key.Count == 2).ToList();
//                     }
//                     else
//                     {
//                         var tableName = fieldKey[0].Name;
//
//                         narrowedDown = candidates
//                             .Where(key => tableName == GetTableNameOfFieldKey(key, evaluator))
//                             .ToList();
//                     }
//
//                     if (narrowedDown.Count == 0)
//                     {
//                         //ToDO: Log Missing field for received data
//                     }
//                     else if (narrowedDown.Count > 1)
//                     {
//                         //TODO: Log Ambiguous name ...
//                         
//                         narrowedDown.Sort(new ClosestLengthFieldKeyComparer());
//                         
//                         map.Add(fieldId, new FieldProfile(narrowedDown[0], evaluator.Map.NodeByKey(narrowedDown[0])));
//                     }
//                     else
//                     {
//                         // exactly 1
//                         map.Add(fieldId, new FieldProfile(narrowedDown[0], evaluator.Map.NodeByKey(narrowedDown[0])));
//                     }
//                 }
//             }
//
//             return map;
//         }
//
//         private class ClosestLengthFieldKeyComparer : IComparer<FieldKey>
//         {
//             public int Compare(FieldKey x, FieldKey y)
//             {
//                 return x.Count - y.Count;
//             }
//         }
//         
//         private List<string> EnumerateFieldIds(List<Record> storageData)
//         {
//             List<string> fieldIds = new List<string>();
//
//             foreach (var record in storageData)
//             {
//                 foreach (var dataPoint in record)
//                 {
//                     var fieldId = dataPoint.Identifier;
//
//                     if (!fieldIds.Contains(fieldId))
//                     {
//                         fieldIds.Add(fieldId);
//                     }
//                 }
//             }
//
//             return fieldIds;
//         }
//
//         private Dictionary<FieldKey, object> IndexByKey(List<DataPoint> standardData, ObjectEvaluator evaluator)
//         {
//             var keyIndexedData = new Dictionary<FieldKey, object>();
//
//             foreach (var data in standardData)
//             {
//                 var key = evaluator.Map.FieldKeyByAddress(data.Identifier);
//
//                 keyIndexedData.Add(key, data.Value);
//             }
//
//             return keyIndexedData;
//         }
//
//         private Dictionary<string, int> CountFieldNames(ICollection<FieldKey> keys)
//         {
//             var counts = new Dictionary<string, int>();
//
//             foreach (var key in keys)
//             {
//                 var id = key.Last().Name;
//
//                 if (!counts.ContainsKey(id))
//                 {
//                     counts.Add(id, 0);
//                 }
//
//                 counts[id]++;
//             }
//
//             return counts;
//         }
//     }
// }