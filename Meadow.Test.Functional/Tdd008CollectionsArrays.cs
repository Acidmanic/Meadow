using System;
using System.Collections;
using System.Collections.Generic;
using Meadow.Reflection;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd008CollectionsArrays:MeadowFunctionalTest
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
            
        }
    }
}