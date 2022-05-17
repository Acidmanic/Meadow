using System;
using System.Collections.Generic;

namespace Meadow.BuildupScripts
{
    public class ScriptInfoAscendingComparer:IComparer<ScriptInfo>
    {
        public int Compare(ScriptInfo x, ScriptInfo y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.OrderIndex.CompareTo(y.OrderIndex);
        }
    }
}