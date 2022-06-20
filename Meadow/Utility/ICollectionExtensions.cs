using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Meadow.Utility
{
    public static class CollectionExtensions
    {
        public static Dictionary<T, int> Count<T>(this ICollection<T> items)
        {
            var counts = new Dictionary<T, int>();

            foreach (var item in items)
            {
                if (!counts.ContainsKey(item))
                {
                    counts.Add(item, 0);
                }

                counts[item]++;
            }

            return counts;
        }
    }
}