using System;
using Acidmanic.Utilities.Reflection;
using Meadow.Test.Functional.Models.BugCase;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd15TypeCheckMustRecognizeCyclicReferences : MeadowFunctionalTest
    {
        private class NonCyclicOuter
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class CyclicOuter
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public CyclicMiddle MiddleGuy { get; set; }
        }

        private class CyclicMiddle
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public CyclicInner InnerGuy { get; set; }
        }

        private class CyclicInner
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public CyclicOuter TheOuterFella { get; set; }
        }

        public override void Main()
        {
            // Check<Person>();
            //
            // Check<CyclicOuter>();
            //
            // Check<NonCyclicOuter>();
            //
            // Check<CyclicMiddle>();
            //
            Check<ProductClassDal>();
            
            // Check<ProductClassPropertyTag>();
            //
            // Check<PropertyDal>();
            //
            // Check<PropertyTypeDal>();
            //
            // Check<SupplementDal>();

        }


        private void Check<T>()
        {
            var type = typeof(T);

            var cyclic = TypeCheck.HasCyclicReferencedDescendants(type);
            
            Console.WriteLine($"{type.Name} is {(cyclic?"":"NOT")} Cyclic Referenced.");
        }
    }
}