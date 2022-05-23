using System;
using System.Collections.Generic;
using System.Reflection;
using Meadow.Reflection.ObjectTree;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Test.Functional.TestDoubles;

namespace Meadow.Test.Functional
{
    public class Tdd010TreeFullRead:MeadowFunctionalTest
    {
        
        
        public override void Main()
        {
            var node  = new TypeAnalyzer().ToAccessNode<List<Person>>();
            
            new AccessNodePrinter().Print(node);

            Console.WriteLine();
            
            var dataStream = new FullTreePersonDataStream();

            foreach (var dp in dataStream)
            {
                PrintObject(dp);
            }
        }
    }
}