using System;
using System.Collections.Generic;

namespace Meadow.Extensions;

public static  class EnumerableExtensions
{



    public static List<TProperty> Aggregate<TModel, TProperty>(this IEnumerable<TModel> sources,
        Func<TModel, IEnumerable<TProperty>> select)
    {

        var aggregatedResult = new List<TProperty>();


        foreach (var source in sources)
        {
            aggregatedResult.AddRange(select(source));
        }

        return aggregatedResult;
    }
}