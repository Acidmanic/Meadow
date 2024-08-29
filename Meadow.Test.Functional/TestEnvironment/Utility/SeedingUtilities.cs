using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Meadow.Attributes;
using Meadow.Requests.BuiltIn;
using Meadow.Requests.GenericEventStreamRequests;

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
        var modelType = typeof(T);

        Func<T, object?> perform = i => PerformCrudInsertion(engine, i);

        if (modelType.GetCustomAttribute<EventStreamPreferencesAttribute>() is { } pref)
        {
            var guidStreamIdForSeeds = "91365d16-85ed-415e-84df-5e56d8870344";
            long longStreamIdForSeeds = 9386229511;
            int intStreamIdForSeeds = 93862295;

            var genericPerformMethod = typeof(SeedingUtilities)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(m => m.Name == nameof(PerformEventInsertion))
                .Where(m => m.IsGenericMethod)
                .FirstOrDefault(m => m.GetGenericArguments().Length == 3)!;

            var performMethod = genericPerformMethod.MakeGenericMethod(typeof(T), pref.EventId, pref.StreamIdType)!;


            if (pref.StreamIdType == typeof(string))
            {
                perform = i =>
                {
                    try
                    {
                        return performMethod.Invoke(null, new object?[] { engine, i, guidStreamIdForSeeds });
                    }
                    catch
                    {
                        /* ignore */
                    }

                    return default;
                };
            }
            else if (pref.StreamIdType == typeof(Guid))
            {
                perform = i =>
                {
                    try
                    {
                        return performMethod.Invoke(null,
                            new object?[] { engine, i, Guid.Parse(guidStreamIdForSeeds) });
                    }
                    catch
                    {
                        /* ignore */
                    }

                    return default;
                };
            }
            else if (pref.StreamIdType == typeof(long))
            {
                perform = i =>
                {
                    try
                    {
                        return performMethod.Invoke(null, new object?[] { engine, i, longStreamIdForSeeds });
                    }
                    catch
                    {
                        /* ignore */
                    }

                    return default;
                };
            }
            else if (pref.StreamIdType == typeof(int))
            {
                perform = i =>
                {
                    try
                    {
                        return performMethod.Invoke(null, new object?[] { engine, i, intStreamIdForSeeds });
                    }
                    catch
                    {
                        /* ignore */
                    }

                    return default;
                };
            }

            var idLeaf = TypeIdentity.FindIdentityLeaf(typeof(T));

            Action<object, object> setId = (i, s) => { };

            if (idLeaf != null)
            {
                setId = (i, s) => idLeaf.Evaluator.Write(s, idLeaf.Evaluator.Read(i));
            }

            foreach (var item in seed)
            {
                var inserted = perform(item);

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

    private static object? PerformCrudInsertion<T>(MeadowEngine engine, T item) where T : class, new()
        => engine.PerformRequest(new InsertRequest<T>(item))
            .FromStorage.FirstOrDefault();

    private static object? PerformEventInsertion<T, TEvId, TStId>(MeadowEngine engine, T item, TStId streamId)
        => engine.PerformRequest(new AppendEventToStreamRequest<T, TEvId, TStId>(streamId, item))
            .FromStorage.FirstOrDefault();
}