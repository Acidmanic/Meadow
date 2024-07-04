using System;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd029ExpressionToFieldKey : MeadowFunctionalTest
    {
        private class MostInner
        {
            public long Id { get; set; }
        }

        private class InnerModel
        {
            public long Id { get; set; }

            public MostInner MostInner { get; set; }
        }

        private class OuterModel
        {
            public long Id { get; set; }

            public InnerModel Inner { get; set; }
        }


        public override void Main()
        {
            var mp = new FiledManipulationMarker();

            mp.Exclude((OuterModel o) => o.Inner.MostInner.Id);

            Console.WriteLine("Inner.MostInner.Id is included? " + mp.IsIncluded((OuterModel o) => o.Inner.MostInner.Id));
            Console.WriteLine("Inner.Id is included? " + mp.IsIncluded((OuterModel o) => o.Inner.Id));
        }
    }
}