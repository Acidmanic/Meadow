// using System;
// using System.Collections.Generic;
// using Acidmanic.Utilities.Reflection.ObjectTree;
// using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
// using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
// using Meadow.Contracts;
// using Meadow.Extensions;
// using Meadow.Utility;
//
// namespace Meadow.Sql
// {
//     public class FieldAddressTranslatedStandardDataTranslator : IStandardDataTranslator
//     {
//         public FieldAddressTranslatedStandardDataTranslator(IFieldAddressIdentifierTranslator translator)
//         {
//             Translator = translator;
//         }
//
//         private class DpComparator : ComparatorBase<DataPoint>
//         {
//             private readonly AddressKeyNodeMap _addressKeyNodeMap;
//
//             public DpComparator(AddressKeyNodeMap addressKeyNodeMap)
//             {
//                 _addressKeyNodeMap = addressKeyNodeMap;
//             }
//
//             protected override int CompareNotNull(DataPoint x, DataPoint y)
//             {
//                
//                 var nx = x == null ? null : _addressKeyNodeMap.NodeByAddress(x.Identifier);
//                 var ny = y == null ? null : _addressKeyNodeMap.NodeByAddress(y.Identifier);
//                 
//                 
//                 if (nx == null && ny == null)
//                 {
//                     return 0;
//                 }
//
//                 if (nx == null)
//                 {
//                     return -1;
//                 }
//
//                 if (ny == null)
//                 {
//                     return 1;
//                 }
//                 return new AccessNodeComparator().Compare(nx,ny);
//             }
//         }
//
//         private  IFieldAddressIdentifierTranslator Translator { get; }
//
//
//         public List<DataPoint> TranslateToStorage(Record standardData, ObjectEvaluator evaluator)
//         {
//             var map = Translator
//                 .MapAddressesByIdentifier(evaluator.RootNode.Type,false)
//                 .Reverse();
//
//
//             var translated = new Record();
//
//             foreach (var data in standardData)
//             {
//                 var keyToLook = FieldKey.Parse(data.Identifier).UnIndexAll();
//
//                 if (map.ContainsKey(keyToLook))
//                 {
//                     var id = map[keyToLook];
//
//                     var value = data.Value;
//
//                     translated.Add(id, value);
//                 }
//             }
//
//             return translated;
//         }
//
//         public List<Record> TranslateFromStorage(List<Record> storageData, Type targetType,bool fullTree)
//         {
//             var map = Translator.MapAddressesByIdentifier(targetType,fullTree:fullTree);
//
//             var translated = new Record();
//
//             var addressKeyNodeMap = new ObjectEvaluator(targetType).Map;
//
//             var recordAccumulator = new SqlRecordAccumulator(addressKeyNodeMap);
//
//             var datapointComparator = new DpComparator(addressKeyNodeMap);
//
//             foreach (var record in storageData)
//             {
//                 var translatedRecord = new Record();
//                 
//                 foreach (var data in record)
//                 {
//                     FieldKey key = null;
//
//                     if (map.ContainsKey(data.Identifier))
//                     {
//                         key = map[data.Identifier];
//                     }
//
//                     var idKey = FieldKey.Parse(data.Identifier).UnIndexAll();
//
//                     while (key == null && idKey.Count > 0)
//                     {
//                         idKey = idKey.Subkey(1, idKey.Count - 1);
//
//                         var id = idKey.ToString();
//
//                         if (map.ContainsKey(id))
//                         {
//                             key = map[id];
//                         }
//                     }
//
//                     if (key == null)
//                     {
//                         Console.WriteLine($"Unable to find a field for {data.Identifier}");
//                     }
//                     else
//                     {
//                         translatedRecord.Add(key.ToString(),data.Value);
//                     }
//                 }
//                 
//                 translatedRecord.Sort(datapointComparator);
//                 
//                 translatedRecord.ForEach( dp =>
//                 {
//                     var k = FieldKey.Parse(dp.Identifier);
//                     recordAccumulator.Pass(dp.Value, new FieldProfile
//                     {
//                         Key = k,
//                         Node = addressKeyNodeMap.NodeByKey(k)
//                     });
//                 });
//             }
//
//             return recordAccumulator.StandardRecords;
//         }
//         
//         // public List<Record> TranslateFromStorage(List<Record> storageData, Type targetType,bool fullTree)
//         // {
//         //     var map = Translator.MapAddressesByIdentifier(targetType,fullTree:fullTree);
//         //
//         //     var translated = new Record();
//         //
//         //     var addressKeyNodeMap = new ObjectEvaluator(targetType).Map;
//         //
//         //     var recordAccumulator = new SqlRecordAccumulator(addressKeyNodeMap);
//         //
//         //     var datapointComparator = new DpComparator(addressKeyNodeMap);
//         //
//         //     foreach (var record in storageData)
//         //     {
//         //         record.Sort(datapointComparator);
//         //
//         //         foreach (var data in record)
//         //         {
//         //             FieldKey key = null;
//         //
//         //             if (map.ContainsKey(data.Identifier))
//         //             {
//         //                 key = map[data.Identifier];
//         //             }
//         //
//         //             var idKey = FieldKey.Parse(data.Identifier).UnIndexAll();
//         //
//         //             while (key == null && idKey.Count > 0)
//         //             {
//         //                 idKey = idKey.Subkey(1, idKey.Count - 1);
//         //
//         //                 var id = idKey.ToString();
//         //
//         //                 if (map.ContainsKey(id))
//         //                 {
//         //                     key = map[id];
//         //                 }
//         //             }
//         //
//         //             if (key == null)
//         //             {
//         //                 Console.WriteLine($"Unable to find a field for {data.Identifier}");
//         //             }
//         //             else
//         //             {
//         //                 recordAccumulator.Pass(data.Value, new FieldProfile
//         //                 {
//         //                     Key = key,
//         //                     Node = addressKeyNodeMap.NodeByKey(key)
//         //                 });
//         //             }
//         //         }
//         //     }
//         //
//         //     return recordAccumulator.StandardRecords;
//         // }
//     }
// }