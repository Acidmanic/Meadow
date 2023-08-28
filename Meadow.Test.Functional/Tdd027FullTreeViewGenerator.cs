// using System;
// using System.Collections.Generic;
// using System.IO;
// using Acidmanic.Utilities.Reflection.ObjectTree;
// using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
// using Meadow.RelationalTranslation;
// using Meadow.Sql;
// using Meadow.Test.Functional.Models.BugCase;
// using Meadow.Test.Functional.TDDAbstractions;
// using Meadow.Tools.Assistant.Extensions;
//
// namespace Meadow.Test.Functional
// {
//     public class Tdd027FullTreeViewGenerator : MeadowFunctionalTest
//     {
//         public override void Main()
//         {
//             
//           //var view = new FullTreeViewGenerator().GetViewDefinition<SupplementDal>();
//
//           //var view = new FullTreeViewGenerator().GetViewStructure<SupplementDal>();
//           
//           var view = new RelationalTranslator()
//               .Initialize(new PluralDataOwnerNameProvider(), '_')
//               .GetFullTreeTable<AutobidStrategyDal>();
//
//           Console.WriteLine(ToView(view));
//         }
//
//         private string ToView(TableDouble table)
//         {
//
//             string view = "CREATE VIEW " + table.Name + " AS \n  SELECT" ;
//             
//             table.Fields.ForEach(f => view += "\n\t" + f);
//
//             view += "\n\t" + table.From;
//             
//             table.Joins.ForEach( j => view += "\n\t" + j);
//
//             return view;
//         }
//     }
// }