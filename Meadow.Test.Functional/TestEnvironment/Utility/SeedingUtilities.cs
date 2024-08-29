using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Meadow.Requests.BuiltIn;
using Meadow.Test.Functional.GenericRequests;

namespace Meadow.Test.Functional.TestEnvironment.Utility;

public static class SeedingUtilities
{
    public static void SeedDataSets(MeadowEngine engine, List<List<object>> data)
    {
        foreach (var items in data)
        {
            SeedByType(engine, items);
        }
    }
    
    public static void SeedByType(MeadowEngine engine, IEnumerable<object> seed)
    {
        var seedList = seed.Where(o => o is { }).Select(o => o!).ToList();

        var modelType = seedList.First().GetType();

        var seedMethod = typeof(SeedingUtilities)
            .GetMethods()
            .Where(m => m.Name == nameof(Seed))
            .Where(m => m.IsGenericMethod)
            .FirstOrDefault(m => m.GetGenericArguments().Length == 1)!;


        var castedSeedList = typeof(List<>).MakeGenericType(modelType).GetConstructor((Type[])new Type[] { })!
            .Invoke(new object[] { });

        var addMethod = castedSeedList.GetType().GetMethod(nameof(IList.Add), new Type[] { modelType })!;

        foreach (var o in seedList)
        {
            addMethod.Invoke(castedSeedList, new object[] { o });
        }

        var method = seedMethod.MakeGenericMethod(modelType);

        method.Invoke(null, new[] { engine, castedSeedList });
    }
    
    
    public static void Seed<T>(MeadowEngine engine, IEnumerable<T> seed) where T : class, new()
    {
        var idLeaf = TypeIdentity.FindIdentityLeaf(typeof(T));

        Action<T, T> setId = (i, s) => { };
        if (idLeaf != null)
        {
            setId = (i, s) => idLeaf.Evaluator.Write(s, idLeaf.Evaluator.Read(i));
        }

        foreach (var item in seed)
        {
            var inserted = engine.PerformRequest(new InsertRequest<T>(item))
                .FromStorage.FirstOrDefault();

            if (inserted == null)
            {
                Console.WriteLine("PROBLEM SEEDING OBJECT");
            }
            else
            {
                setId(inserted, item);
            }
        }
    }
}