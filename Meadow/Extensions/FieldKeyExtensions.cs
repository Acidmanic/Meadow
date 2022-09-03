using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Extensions
{
    public static class FieldKeyExtensions
    {
        public static string ToString(this FieldKey key, char delimiter)
        {
            var result = "";

            var sep = "";

            foreach (var segment in key)
            {
                result += sep + segment.Name;

                sep = delimiter +"";
            }

            return result;
        }
        
        public static FieldKey Subkey(this FieldKey key, int index, int length)
        {
           var result = new FieldKey();

           for (int i = index; i < index + length; i++)
           {
               var segment = key[i].Clone();
               
               result.Add(segment);
           }

           return result;
        }
        
        public static FieldKey UnIndexAll(this FieldKey key)
        {
            var result = new FieldKey();

            foreach (var segment in key)
            {
                result.Add(new Segment(segment.Name));
            }

            return result;
        }
        
        public static FieldKey ClearIndexes(this FieldKey key)
        {
            var result = new FieldKey();

            foreach (var segment in key)
            {
                if (segment.Indexed)
                {
                    result.Add(new Segment(segment.Name,-1));    
                }
                else
                {
                    result.Add(new Segment(segment.Name));
                }
                
            }

            return result;
        }
        
        public static FieldKey ZeroIndexes(this FieldKey key)
        {
            var result = new FieldKey();

            foreach (var segment in key)
            {
                if (segment.Indexed)
                {
                    result.Add(new Segment(segment.Name,0));    
                }
                else
                {
                    result.Add(new Segment(segment.Name));
                }
                
            }

            return result;
        }
    }
}