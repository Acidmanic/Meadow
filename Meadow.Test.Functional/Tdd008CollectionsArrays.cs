using System;
using System.Collections;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.Sets;
using Meadow.Reflection.Sets;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd008CollectionsArrays : MeadowFunctionalTest
    {
        public override void Main()
        {
            object array = new string[] { "first","second"};
            
            if (array is Array arSrc)
            {
                
                var arCol = new ArrayCollection(arSrc);
                
                arCol.Add("Mona");
            
                var ar = arCol.WrappedArray;
            
                foreach (var e in arCol)
                {
                    Console.WriteLine(e);
                }
            }
            
            var person = new ObjectInstantiator().CreateObject<Person>(true);
            
            PrintObject(person);


            object realCollection = new List<string>
            {
                "First", "Second"
            };

            if (realCollection is ICollection collection)
            {
                var colCol = new CollectionCollection(collection);
                
                colCol.Add("Acidmanic");

                colCol.Remove("Second");
                
                foreach (var item in colCol)
                {
                    Console.WriteLine(item);
                }
            }
        }
    }
}