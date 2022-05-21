using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Channels;
using Meadow.Reflection;
using Meadow.Reflection.ObjectTree;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public static class FlatMapExtension
    {
        public static void Print(this FlatMap map, object root)
        {
            map.WalkThrough((f, get, set) =>
            {
                var value = get(root);

                Console.WriteLine(f + ": \t" + value);
            });
        }
    }

    public class Tdd005FlatteningTests : MeadowFunctionalTest
    {
        private class OuterMost
        {
            public long Id { get; set; }

            public string Name { get; set; }

            public int MiddleId { get; set; }

            public Middle Middle { get; set; }
        }

        private class Middle
        {
            public int Id { get; set; }

            public string Title { get; set; }

            public string InnerMostId { get; set; }

            public InnerMost InnerMost { get; set; }
        }

        private class InnerMost
        {
            public string Id { get; set; }

            public string Name { get; set; }
        }


        public override void Main()
        {
            var analyzer = new TypeAnalyzer();

            var flatMap = analyzer.Map<OuterMost>();

            var data1 = new OuterMost
            {
                Id = 1,
                Middle = new Middle
                    {Id = 2, InnerMost = new InnerMost {Id = "im3", Name = "i1"}, Title = "tit1", InnerMostId = "im3"},
                Name = "out1", MiddleId = 2
            };
            var data2 = new OuterMost
            {
                Id = 10,
                Middle = new Middle
                {
                    Id = 20, InnerMost = new InnerMost {Id = "im30", Name = "i10"}, Title = "tit10",
                    InnerMostId = "im30"
                },
                Name = "out10", MiddleId = 20
            };

            flatMap.Print(data1);


            flatMap.WalkThrough((f, g, s) =>
            {
                var bigValue = g(data2);
                s(data1, bigValue);
            });

            Console.WriteLine("After overwrite: ");

            flatMap.Print(data1);
        }
    }
}