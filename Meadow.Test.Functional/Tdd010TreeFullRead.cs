using System.Collections.Generic;
using Meadow.Reflection.ObjectTree;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd010TreeFullRead:MeadowFunctionalTest
    {
        public override void Main()
        {
            var node  = new TypeAnalyzer().ToAccessNode<List<Person>>();
            
            new AccessNodePrinter().Print(node);
        }
    }
}