using System;
using Meadow.Scaffolding.Sqlable;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd022RegExTests : MeadowFunctionalTest
    {
        public override void Main()
        {
            var code = " create procedure maniassomething () as somethingElse";

            var namePar1 = code.SubStringBetween("PROCEDURE", "\\sAS", true);

            Console.WriteLine(namePar1 == null ? "null" : namePar1);
        }
    }
}