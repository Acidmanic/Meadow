using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

namespace Meadow.Test.Functional.TestEnvironment;

public static class AssertX
{
    public static void ContainSameItemsShallow<T>(List<T> expectedItems, List<T> actualItems, Func<T, string> toString = null)
    {
        toString ??= (T t) => t.ToString();

        foreach (var item in expectedItems)
        {
            ContainShallow(actualItems, item, $"Item: {toString(item)} was not found in the result");
        }

        AreSameSize(expectedItems, actualItems, "UnExpected extra Items has been found In Actual items list.");
    }
    
    public static void ContainSameItemsDeep<T>(List<T> expectedItems, List<T> actualItems, Func<T, string> toString = null)
    {
        toString ??= (T t) => t.ToString();

        foreach (var item in expectedItems)
        {
            ContainDeep(actualItems, item, $"Item: {toString(item)} was not found in the result");
        }

        AreSameSize(expectedItems, actualItems, "UnExpected extra Items has been found In Actual items list.");
    }


    public static void InSameOrderShallow<T>(List<T> expected, List<T> actual, Func<T, string> toString = null)
    {
        toString ??= (T t) => t.ToString();

        for (int i = 0; i < expected.Count; i++)
        {
            if (!AreEqualShallow(expected[i], actual[i]))
            {
                throw new Exception($"Expected to find {toString(expected[i])} at {i}'th place, but found {toString(actual[i])}");
            }
        }
    }
    
    public static void InSameOrderDeep<T>(List<T> expected, List<T> actual, Func<T, string> toString = null)
    {
        toString ??= (T t) => t.ToString();

        for (int i = 0; i < expected.Count; i++)
        {
            if (!AreEqualDeep(expected[i], actual[i]))
            {
                throw new Exception($"Expected to find {toString(expected[i])} at {i}'th place, but found {toString(actual[i])}");
            }
        }
    }

    public static void AreSameSize<T>(IEnumerable<T> s1, IEnumerable<T> s2, string message)
    {
        if (s1.Count() != s2.Count())
        {
            throw new Exception(message);
        }
    }

    public static void ContainShallow<T>(List<T> found, T person, string message)
    {
        if (found.All(f => !AreEqualShallow(f, person)))
        {
            throw new Exception(message);
        }
    }
    public static void ContainDeep<T>(List<T> found, T person, string message)
    {
        if (found.All(f => !AreEqualDeep(f, person)))
        {
            throw new Exception(message);
        }
    }

    public static bool AreEqualShallow<T>(T p1, T p2, bool ignoreId = true)
        => AreEqual(p1, p2, o => o.DirectLeavesOnly().UseOriginalTypes().ExcludeNulls(), ignoreId);

    public static bool AreEqualDeep<T>(T p1, T p2, bool ignoreId = true)
        => AreEqual(p1, p2, o => o.FullTree().UseOriginalTypes().ExcludeNulls(), ignoreId);

    private static bool AreEqual<T>(T p1, T p2, Action<IStandardConversionOptionsBuilder> o, bool ignoreId = true)
    {
        var tev = new ObjectEvaluator(typeof(T));

        var idLeaves = tev.Map.Nodes.Select(n => n.Type)
            .Where(t => TypeCheck.IsModel(t))
            .Select(TypeIdentity.FindIdentityLeaf)
            .Where(n => n is { })
            .Select(n => n.GetFullName())
            .ToList();

        bool IsId(string n) => idLeaves.Any(i => string.CompareOrdinal(i, n) == 0);

        var e1 = new ObjectEvaluator(p1);
        var e2 = new ObjectEvaluator(p2);

        var s1 = e1.ToStandardFlatData(o);
        var s2 = e2.ToStandardFlatData(o);

        if (s1.Count != s2.Count) return false;

        for (int i = 0; i < s1.Count; i++)
        {
            if (s1[i].Identifier != s2[i].Identifier) return false;

            var identifier = s1[i].Identifier;

            if (!(ignoreId && IsId(identifier)))
            {
                if (!AreEqualObjects(s1[i].Value, s2[i].Value)) return false;
            }
        }

        return true;
    }

    private static bool AreEqualObjects(object o1, object o2)
    {
        if (o1.GetType() != o2.GetType()) return false;

        if (o1 is string os1 && o2 is string os2) return string.Compare(os1, os2, StringComparison.Ordinal) == 0;

        var type = o1.GetType();

        if (TypeCheck.IsNumerical(type))
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return o1.AsNumber() == o2.AsNumber();
        }

        var equalCheck = type.GetMethod("Equals", new Type[] { type });

        if (equalCheck is { } check)
        {
            return (bool)(check.Invoke(o1, new[] { o2 }) ?? false);
        }

        return o1 == o2;
    }
}