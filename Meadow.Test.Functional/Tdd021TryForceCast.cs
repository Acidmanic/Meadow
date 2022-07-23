using System;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd021TryForceCast:MeadowFunctionalTest
    {

        class A
        {
            public int Id { get; set; }
        }
        
        public override void Main()
        {
            var a = new A
            {
                Id = 12
            };

            var propertyInfo = typeof(A).GetProperty("Id");

            long newValue = 32;

            var conversionType = typeof(int);

            var forceCasted = Convert.ChangeType(newValue, conversionType);
            
            propertyInfo.SetValue(a,forceCasted);
            
            PrintObject(a);

        }
    }
}