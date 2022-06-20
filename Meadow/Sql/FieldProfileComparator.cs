using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.Sql
{
    public class FieldProfileComparator:IComparer<FieldProfile>
    {
        private readonly  AccessNodeComparator _comparator = new AccessNodeComparator();
        
        public int Compare(FieldProfile x, FieldProfile y)
        {
            return _comparator.Compare(x?.Node, y?.Node);
        }
    }
}