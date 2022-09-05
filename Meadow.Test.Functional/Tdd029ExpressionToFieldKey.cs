using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Extensions;
using Meadow.RelationalTranslation;
using Meadow.Requests.FieldInclusion;
using Meadow.Sql;
using Meadow.Test.Functional.Models.BugCase;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Tools.Assistant.Extensions;

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
            var mp = new FiledManipulationMarker<OuterModel>(new PluralDataOwnerNameProvider(), true);

            mp.Exclude(o => o.Inner.MostInner.Id);

            Console.WriteLine("Inner.MostInner.Id is included? " + mp.IsIncluded(o => o.Inner.MostInner.Id));
            Console.WriteLine("Inner.Id is included? " + mp.IsIncluded(o => o.Inner.Id));
        }
    }
}