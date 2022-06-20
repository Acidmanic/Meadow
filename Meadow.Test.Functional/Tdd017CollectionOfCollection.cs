// using System.Collections.Generic;
// using Acidmanic.Utilities.Reflection.Attributes;
// using Meadow.Reflection.Mapping;
// using Meadow.Test.Functional.TDDAbstractions;
// using Meadow.Test.Functional.TestDoubles;
//
// namespace Meadow.Test.Functional
// {
//     public class Tdd017X2XRelationTest : MeadowFunctionalTest
//     {
//         public class ModelA
//         {
//             public List<ModelB> BGuys { get; set; }
//
//             public string Name { get; set; }
//             [UniqueMember]
//             public long Id { get; set; }
//         }
//
//         public class ModelB
//         {
//             [UniqueMember]
//             [AutoValuedMember]
//             public string Id
//             {
//                 get
//                 {
//                     return ModelAId.ToString() + ":" + TargetXId.ToString();
//                 }
//                 set
//                 {
//                     //meeh!
//                 }
//             }
//
//             public long ModelAId { get; set; }
//             
//             public long TargetXId { get; set; }
//             
//             public TargetX Target { get; set; }
//         }
//
//
//         public class TargetX
//         {
//             public string Title { get; set; }
//             [UniqueMember]
//             public long Id { get; set; }
//         }
// /*
//  *        Table ModelAs
//  *             Name: string    | Id: long
//  * 
//  *        Table ModelBs
//  *             TargetXId: long        | ModelAId: long
//  *             
//  *
//  *         Record1:
//  *             Id:1,Name:first,ModelBs.TargetXId:1,ModelAId:1
//  *         Record2:
//  *             Id:1,Name:first,ModelBs.TargetXId:2,ModelAId:1
//  */
//         public class Tdd17DataReader : InMemoryDataReader
//         {
//             public Tdd17DataReader()
//             {
//                 CreateRecord()
//                     .InsertField("ModelAs.Id", (long) 1)
//                     .InsertField("Name", "first")
//                     .InsertField("TargetXId", (long) 1)
//                     .InsertField("ModelAId", (long) 1)
//                     .InsertField("TargetXs.Id",(long)1)
//                     .InsertField("Title","FirstTarget")
//                     ;
//
//                 CreateRecord()
//                     .InsertField("ModelAs.Id", (long) 1)
//                     .InsertField("Name", "first")
//                     .InsertField("TargetXId", (long) 2)
//                     .InsertField("ModelAId", (long) 1)
//                     .InsertField("TargetXs.Id",(long)2)
//                     .InsertField("Title","SecondTarget");
//
//                 CreateRecord()
//                     .InsertField("ModelAs.Id", (long) 2)
//                     .InsertField("Name", "second");
//             }
//         }
//
//         public override void Main()
//         {
//             var writer = new ObjectDataWriter(typeof(List<ModelA>), true);
//
//             var dataReader = new Tdd17DataReader();
//
//             writer.WriteIntoRootObject(dataReader,true);
//
//             var result = writer.RootObject;
//
//             PrintObject(result);
//         }
//     }
// }