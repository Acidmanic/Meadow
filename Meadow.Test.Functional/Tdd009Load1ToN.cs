using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Meadow.Reflection.ObjectTree;
using Meadow.Test.Functional.Requests;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd009Load1ToN:MeadowFunctionalTest
    {
        public Tdd009Load1ToN():base("MeadowScratch")
        {
            
        }

        private class OuterClass
        {
            public List<CollectedClass> BecheMecheHa { get; set; }
        }

        private class CollectedClass
        {
            public string Name { get; set; }
        }
        
        public override void Main()
        {
            var engine = SetupClearDatabase();
            
            var request = new GetAllPersonsFullTree();
            
            var result = engine.PerformRequest(request);
            
            PrintObject(result.FromStorage);

            // var map = new TypeAnalyzer().Map<OuterClass>(true);
            //
            // var blank = new TypeAnalyzer().CreateObject<OuterClass>(true);
            //
            // map.WriteIntoRootObject("Name",blank,"Mani");
            //
            // PrintObject(blank);
            
            
            
        }
    }
}