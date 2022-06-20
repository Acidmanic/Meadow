// using System;
// using System.Collections.Generic;
// using System.Data;
// using Acidmanic.Utilities.Reflection.ObjectTree;
// using Meadow.Reflection.Mapping;
// using Meadow.Test.Functional.Models;
// using Meadow.Test.Functional.TDDAbstractions;
// using Meadow.Test.Functional.TestDoubles;
//
// namespace Meadow.Test.Functional
// {
//     public class Tdd010TreeFullRead : MeadowFunctionalTest
//     {
//         public class DataSet<TModel>
//         {
//             public List<TModel> Data { get; set; }
//         }
//
//         public override void Main()
//         {
//             var node = new TypeAnalyzer().ToAccessNode<List<Person>>(true);
//
//             new AccessNodePrinter().Print(node);
//
//             Console.WriteLine();
//
//             var dataReader = new FullTreePersonDataReader();
//             
//             var machine = new ObjectDataWriter(node);
//             
//             machine.WriteIntoRootObject(dataReader);
//             
//             var chos = machine.RootObject;
//             
//             PrintObject(chos);
//         }
//
//         private List<string> EnumFields(IDataReader dataReader)
//         {
//             var result = new List<string>();
//
//             for (int i = 0; i < dataReader.FieldCount; i++)
//             {
//                 result.Add(dataReader.GetName(i));
//             }
//
//             return result;
//         }
//     }
// }