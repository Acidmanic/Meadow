using System;
using System.Collections.Generic;

namespace Meadow.Utility
{
    public abstract class ComparatorBase<T> : IComparer<T>
    {
        protected class SubCompareResult
        {
            public int Comparison { get; set; }

            public bool XApplies { get; set; }

            public bool YApplies { get; set; }

            public SubCompareResult SetComparison(int c)
            {
                this.Comparison = c;

                return this;
            }

            public bool WereEqual => XApplies == YApplies;
        }

        public int Compare(T x, T y)
        {
            var subCompares = SubCompare(o => o == null, x, y);

            if (subCompares.WereEqual)
            {
                return CompareNotNull(x, y);
            }

            return subCompares.Comparison;
        }


        protected abstract int CompareNotNull(T x, T y);

        protected SubCompareResult SubCompare(Func<T, bool> appies, T x, T y, int direction = 1)
        {
            var ax = appies(x);
            var ay = appies(y);

            var result = new SubCompareResult {XApplies = ax, YApplies = ay};

            if (ax == ay)
            {
                return result.SetComparison(0);
            }

            if (ax)
            {
                return result.SetComparison(direction);
            }

            return result.SetComparison(-direction);
        }
    }
}