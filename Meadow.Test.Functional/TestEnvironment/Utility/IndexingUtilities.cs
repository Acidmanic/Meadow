#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Meadow.Requests.BuiltIn;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Transliteration;
using Meadow.Transliteration.Builtin;

namespace Meadow.Test.Functional.TestEnvironment.Utility;

public static class IndexingUtilities
{
    public static void Index<T>(MeadowEngine engine, IEnumerable<T> seed,
        ITransliterationService transliterationService)
    {
        var idLeaf = TypeIdentity.FindIdentityLeaf<T>();
        var idType = idLeaf.Type;
        var requestGeneric = typeof(IndexEntity<,>);
        var requestType = requestGeneric.MakeGenericType(typeof(T), idType);
        var constructor = requestType.GetConstructor(new Type[] { typeof(string), idType });
        var indexing = new IndexCorpusService<T>(transliterationService!);
        var genericSearchIndex = typeof(SearchIndex<>);
        var searchIndexType = genericSearchIndex.MakeGenericType(idType);
        var methodName = nameof(MeadowEngine.PerformRequest);
        var genericPerformMethod = typeof(MeadowEngine)
            .GetMethods()
            .FirstOrDefault(m => m.Name == methodName &&
                                 m.GetGenericArguments().Length == 2);
        var performMethod = genericPerformMethod!.MakeGenericMethod(searchIndexType, searchIndexType);
        foreach (var item in seed)
        {
            var corpus = indexing.GetIndexCorpus(item, false);
            var id = idLeaf.Evaluator.Read(item);
            var request = constructor!.Invoke(new object[] { corpus, id });

            performMethod!.Invoke(engine, new object[] { request, false });
        }
    }

    public static void Index<T>(MeadowEngine engine, IEnumerable<T> seed, bool fullTreeIndexing = true)
    {
        var idLeaf = TypeIdentity.FindIdentityLeaf<T>();
        var idType = idLeaf.Type;
        var requestGeneric = typeof(IndexEntity<,>);
        var requestType = requestGeneric.MakeGenericType(typeof(T), idType);
        var constructor = requestType.GetConstructor(new Type[] { typeof(T), typeof(bool) });
        var methodName = nameof(MeadowEngine.PerformRequest);
        var genericPerformMethod = typeof(MeadowEngine)
            .GetMethods()
            .FirstOrDefault(m => m.Name == methodName && m.GetGenericArguments().Length == 2);

        var genericSearchIndex = typeof(SearchIndex<>);
        var searchIndexType = genericSearchIndex.MakeGenericType(idType);

        var performMethod = genericPerformMethod!.MakeGenericMethod(searchIndexType, searchIndexType);
        foreach (var item in seed)
        {
            if (item is { } itemValue)
            {
                var request = constructor!.Invoke(new object[] { itemValue, fullTreeIndexing });

                performMethod!.Invoke(engine, new object[] { request, false });
            }
        }
    }
}