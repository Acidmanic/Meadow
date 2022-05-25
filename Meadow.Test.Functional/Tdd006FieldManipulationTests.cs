using System;
using Meadow.Attributes;
using Meadow.Reflection.ObjectTree;
using Meadow.Requests;
using Meadow.Requests.FieldManipulation;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd006FieldManipulationTests:MeadowFunctionalTest
    {

        public class A
        {
            public long Id { get; set; }
            
            public string Name { get; set; }
            
            public long BId { get; set; }
            
            public B B { get; set; }
        }

        public class B
        {
            public long Id { get; set; }            
            public C CItem { get; set; }
            [Field("CId")]
            public int CItemId { get; set; }
        }

        [Table("CTable")]
        public class C
        {
            public int Id { get; set; }
            [Field("CTitle")]
            public string Name { get; set; }
            
            public string StupidProperty { get; set; }
        }
        
        private A model = new A
        {
            Id = 1,
            Name = "A-Name",
            BId = 2,
            B = new B
            {
                Id = 2,
                CItemId = 3,
                CItem = new C
                {
                    Id = 3,
                    Name = "C-Name-C"
                }
            }
        };
        public override void Main()
        {
            var marker = new FiledManipulationMarker<A>();

            marker
                .Exclude(a => a.B.CItem.StupidProperty)
                .Rename(a => a.BId, "BaId");
            
            var flatMap = new TypeAnalyzer().Map<A>(true);
            
            flatMap.WalkThrough((f, get, set) =>
            {
                var line = f + "\t";

                if (marker.IsIncluded(f))
                {
                    line += marker.GetPracticalName(f);
                }
                else
                {
                    line += "X-cluded-X";
                }
                Console.WriteLine(line);
            });
        }
    }
}