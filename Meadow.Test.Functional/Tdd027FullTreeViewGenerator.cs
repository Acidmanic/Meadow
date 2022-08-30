using System;
using System.Collections.Generic;
using System.IO;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Sql;
using Meadow.Test.Functional.Models.BugCase;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Tools.Assistant.Extensions;

namespace Meadow.Test.Functional
{
    public class Tdd027FullTreeViewGenerator : MeadowFunctionalTest
    {
        public override void Main()
        {
            
          //var view = new FullTreeViewGenerator().GetViewDefinition<SupplementDal>();

          var view = new FullTreeViewGenerator().GetViewStructure<SupplementDal>();

          Console.WriteLine(view);
        }
    }
}