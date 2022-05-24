using System.Collections.Generic;

namespace Meadow.Reflection.ObjectTree
{
    public class AccessNodeComparator : Comparer<AccessNode>
    {
        public override int Compare(AccessNode x, AccessNode y)
        {
            var unique1 = x.IsUnique;

            var unique2 = y.IsUnique;


            if (unique1 == unique2)
            {
                return Diff(x.Depth, y.Depth);
            }

            return unique1 ? -1 : 1;
        }

        private int Diff(int v1, int v2)
        {
            if (v1 == v2)
            {
                return 0;
            }

            if (v1 > v2)
            {
                return 1;
            }

            return -1;
        }
    }
}