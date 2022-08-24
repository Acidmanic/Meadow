using System.Collections.Generic;

namespace Meadow.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TValue, TKey> Reverse<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            var reversed = new Dictionary<TValue, TKey>();


            foreach (var item in dictionary)
            {
                reversed.Add(item.Value, item.Key);
            }

            return reversed;
        }
    }
}