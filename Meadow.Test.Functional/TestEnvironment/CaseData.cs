using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.Test.Functional.TestEnvironment;

public class CaseData
{


    public IReadOnlyDictionary<Type, List<object>> SeedsByType => _seedsByType;
    
    private readonly Dictionary<Type, List<object>> _seedsByType;


    private CaseData(Dictionary<Type, List<object>> seedsByType)
    {
        _seedsByType = seedsByType;
    }

    public List<T> Get<T>()
    {
        if (_seedsByType.ContainsKey(typeof(T)))
        {
            return new List<T>(_seedsByType[typeof(T)].Select(o => (T)o));
        }

        return new List<T>();
    }

    public List<T> Get<T>(Func<T, bool> predicate) => Get<T>().Where(predicate).ToList();
    

    public List<T> Get<T>(Comparison<T> comparison) => Sort(Get<T>(), comparison);


    private List<TModel> Sort<TModel>(IEnumerable<TModel> items, Comparison<TModel> compare)
    {
        var list = new List<TModel>(items);

        list.Sort(compare);

        return list;
    }
    
    
    public static CaseData Create(ICaseDataProvider provider)
    {
        
        provider.Initialize();

        var dataSets = provider.SeedSet;

        return Create(dataSets);
    }
    
    public static CaseData Create(List<List<object>> dataSets)
    {
        var seedsByType = new Dictionary<Type, List<object>>();
        
        foreach (var dataSet in dataSets)
        {
            if (dataSet.Count > 0)
            {
                var type = dataSet.First(d => d is { }).GetType();

                if (!seedsByType.ContainsKey(type))
                {
                    seedsByType.Add(type, new List<object>());
                }

                foreach (var o in dataSet)
                {
                    seedsByType[type].Add(o);
                }
            }
        }

        return new CaseData(seedsByType);
    }
    
}