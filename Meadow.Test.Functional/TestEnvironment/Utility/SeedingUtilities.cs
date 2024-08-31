using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Meadow.Attributes;
using Meadow.Requests.BuiltIn;
using Meadow.Requests.GenericEventStreamRequests;
using Meadow.Requests.GenericEventStreamRequests.Models;

namespace Meadow.Test.Functional.TestEnvironment.Utility;

public static class SeedingUtilities
{
    public static void SeedCaseData(MeadowEngine engine, CaseData caseData)
    {
        foreach (var crudSeed in caseData.SeedsByType)
        {
            SeedCrudByType(engine, crudSeed.Value, crudSeed.Key);
        }

        foreach (var eventSeed in caseData.Events())
        {
            SeedEvent(engine, eventSeed);
        }
    }

    public static void SeedCrudByType(MeadowEngine engine, IEnumerable<object> seed, Type? modelType = null)
    {
        var seedList = seed.ToList();

        modelType ??= seedList.First().GetType();

        var seedMethod = typeof(SeedingUtilities)
            .GetMethods()
            .Where(m => m.Name == nameof(SeedCrud))
            .Where(m => m.IsGenericMethod)
            .FirstOrDefault(m => m.GetGenericArguments().Length == 1)!;


        var castedSeedList = typeof(List<>).MakeGenericType(modelType).GetConstructor(new Type[] { })!
            .Invoke(new object[] { });

        var addMethod = castedSeedList.GetType().GetMethod(nameof(IList.Add), new[] { modelType })!;

        foreach (var o in seedList)
        {
            addMethod.Invoke(castedSeedList, new[] { o });
        }

        var method = seedMethod.MakeGenericMethod(modelType);

        method.Invoke(null, new[] { engine, castedSeedList });
    }


    public static object? SeedEvent(MeadowEngine engine, StreamEvent streamEvent)
    {
        if (streamEvent.EventConcreteType.GetCustomAttribute<EventStreamPreferencesAttribute>(true) is { } pref)
        {
            if (streamEvent.StreamId is { } streamId && streamId.GetType() == pref.StreamIdType)
            {
                var genericPerformMethod = typeof(SeedingUtilities)
                    .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                    .Where(m => m.Name == nameof(PerformEventInsertion))
                    .Where(m => m.IsGenericMethod)
                    .FirstOrDefault(m => m.GetGenericArguments().Length == 3)!;

                var performMethod = genericPerformMethod.MakeGenericMethod(streamEvent.EventConcreteType, pref.EventId, pref.StreamIdType);

                var inserted = performMethod.Invoke(null, new[] { engine, streamEvent.Event, streamId });

                if (inserted == null)
                {
                    Console.WriteLine("PROBLEM SEEDING OBJECT");
                }

                return inserted;
            }
        }

        return null;
    }


    public static void SeedCrud<T>(MeadowEngine engine, IEnumerable<T> seed) where T : class, new()
    {
        var eventIdLeaf = TypeIdentity.FindIdentityLeaf(typeof(T));

        Action<object, object> setId = (_, _) => { };

        if (eventIdLeaf is { } readerNode)
        {
            setId = (i, s) => eventIdLeaf.Evaluator.Write(s, readerNode.Evaluator.Read(i));
        }

        foreach (var item in seed)
        {
            var inserted = PerformCrudInsertion(engine, item);

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

    private static object? PerformCrudInsertion<T>(MeadowEngine engine, T item) where T : class, new()
        => engine.PerformRequest(new InsertRequest<T>(item))
            .FromStorage.FirstOrDefault();

    private static object? PerformEventInsertion<T, TEvId, TStId>(MeadowEngine engine, T item, TStId streamId)
        => engine.PerformRequest(new AppendEventToStreamRequest<T, TEvId, TStId>(streamId, item))
            .FromStorage.FirstOrDefault();
}