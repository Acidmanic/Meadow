using System;
using System.Collections.Generic;
using System.IO;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Extensions;
using Meadow.Sql;
using Meadow.Test.Functional.Models.Bug2;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Test.Functional.Utility;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public class Tdd37FlagEnumIs : MeadowFunctionalTest
    {
        [Flags]
        private enum Biz
        {
            A = 1,
            B = 2,
            C = 4,
            AB = A | B,
            BC = B | C,
            ABC = A | B | C
        }

        public override void Main()
        {

            Console.WriteLine("ABC is A:" +(Biz.ABC is Biz.A));
            
            
        }
    }
}