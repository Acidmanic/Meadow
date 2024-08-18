using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Acidmanic.Utilities.Reflection.Sets;
using Meadow.Extensions;

namespace Meadow.Test.Functional.TestEnvironment;

public static class AssertX
{
    public static void ContainSameItemsShallow<T>(List<T> expectedItems, List<T> actualItems, Func<T, string> toString = null, bool ignoreId = true)
        => ContainSameItems(expectedItems, actualItems, toString, ignoreId, false);

    public static void ContainSameItemsDeep<T>(List<T> expectedItems, List<T> actualItems, Func<T, string> toString = null, bool ignoreId = true)
        => ContainSameItems(expectedItems, actualItems, toString, ignoreId, true);

    public static void ContainSameItems<T>(List<T> expectedItems, List<T> actualItems, Func<T, string> toString = null, bool ignoreId = true, bool deepCompare = false)
    {
        toString ??= (T t) => t.ToString();

        foreach (var item in expectedItems)
        {
            Contains(actualItems, item, $"Item: {toString(item)} was not found in the result", ignoreId, deepCompare);
        }

        AreSameSize(expectedItems, actualItems, "UnExpected extra Items has been found In Actual items list.");
    }


    public static void AreInSameOrderShallow<T>(List<T> expected, List<T> actual, Func<T, string> toString = null, bool ignoreId = true)
        => AreInSameOrder(expected, actual, toString, ignoreId, false);

    public static void AreInSameOrderDeep<T>(List<T> expected, List<T> actual, Func<T, string> toString = null, bool ignoreId = true)
        => AreInSameOrder(expected, actual, toString, ignoreId, true);

    public static void AreInSameOrder<T>(List<T> expected, List<T> actual, Func<T, string> toString = null, bool ignoreId = true, bool deepCompare = false)
    {
        toString ??= (T t) => t.ToString();

        for (int i = 0; i < expected.Count; i++)
        {
            if (!AreEqual(expected[i], actual[i], ignoreId, deepCompare))
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

    public static void ContainShallow<T>(List<T> found, T person, string message, bool ignoreId = true) => Contains(found, person, message, ignoreId, false);

    public static void ContainsDeep<T>(List<T> found, T person, string message, bool ignoreId = true) => Contains(found, person, message, ignoreId, true);

    public static void Contains<T>(List<T> found, T person, string message, bool ignoreId = true, bool deepCompare = false)
    {
        if (found.All(f => !AreEqual(f, person, ignoreId, deepCompare)))
        {
            throw new Exception(message);
        }
    }

    public static bool AreEqualShallow<T>(T p1, T p2, bool ignoreId = true, bool fullTree = false) => AreEqual(p1, p2, ignoreId, false);

    public static bool AreEqualDeep<T>(T p1, T p2, bool ignoreId = true, bool fullTree = false) => AreEqual(p1, p2, ignoreId, fullTree);

    private static bool AreEqual<T>(T p1, T p2, bool ignoreId = true, bool fullTree = false)
    {
        Action<IStandardConversionOptionsBuilder> options = b =>
        {
            if (fullTree)
            {
                b.FullTree();
            }
            else
            {
                b.DirectLeavesOnly();
            }

            b.UseOriginalTypes().ExcludeNulls();
        };
        var tev = new ObjectEvaluator(typeof(T));

        var idLeaves = tev.Map.Nodes
            .Where(n => TypeCheck.IsModel(n.Type))
            .Select(n => new { Node = n, Id = TypeIdentity.FindIdentityLeaf(n.Type) })
            .Where(n => n is { Node: { }, Id: { } })
            .Select(n => n.Node.GetFullName() + "." + n.Id.Name)
            .ToList();

        bool IsId(string n) => idLeaves.Any(i => string.CompareOrdinal(i, n) == 0);


        var flat1 = Flatten(p1, options);
        var flat2 = Flatten(p2, options);

        if (flat1.Count != flat2.Count) return false;

        foreach (var keyValue in flat1)
        {
            if (!flat2.ContainsKey(keyValue.Key)) return false;

            if (!ignoreId || !IsId(keyValue.Key.ToString()))
            {
                if (!ContainSameObjects(keyValue.Value, flat2[keyValue.Key])) return false;
            }
        }

        return true;
    }

    private static Dictionary<FieldKey, List<object>> Flatten<T>(T value, Action<IStandardConversionOptionsBuilder> options)
    {
        var ev = new ObjectEvaluator(value);

        var standardFlat = ev.ToStandardFlatData(options);

        var result = new Dictionary<FieldKey, List<object>>();

        foreach (var dataPoint in standardFlat)
        {
            var key = FieldKey.Parse(dataPoint.Identifier).ClearIndexes();

            if (!result.ContainsKey(key)) result.Add(key, new List<object>());

            result[key].Add(dataPoint.Value);
        }

        return result;
    }

    private static bool ContainSameItems(object o1, object o2, Type type, bool ignoreId = true, bool fullTree = false)
    {
        if (o1 is IEnumerable { } col1 && o2 is IEnumerable { } col2)
        {
            var list1 = new List<object>();
            var list2 = new List<object>();

            foreach (var i in col1) list1.Add(i);
            foreach (var i in col2) list1.Add(i);

            foreach (var ci in list1)
            {
                if (list2.All(li => !AreEqual2(ci, li, type, ignoreId, fullTree)))
                {
                    return false;
                }
            }

            return list1.Count == list2.Count;
        }

        return o1 is null == o2 is null;
    }

    private static bool AreEqual2(object p1, object p2, Type type, bool ignoreId = true, bool fullTree = false)
    {
        Action<IStandardConversionOptionsBuilder> options = b =>
        {
            if (fullTree)
            {
                b.FullTree();
            }
            else
            {
                b.DirectLeavesOnly();
            }

            b.UseOriginalTypes().ExcludeNulls();
        };

        var tev = new ObjectEvaluator(type);
        var idLeaves = tev.Map.Nodes
            .Where(n => TypeCheck.IsModel(n.Type))
            .Select(n => new { Node = n, Id = TypeIdentity.FindIdentityLeaf(n.Type) })
            .Where(n => n is { Node: { }, Id: { } })
            .Select(n => n.Node.GetFullName() + "." + n.Id.Name)
            .ToList();

        bool IsId(string n) => idLeaves.Any(i => string.CompareOrdinal(i, n) == 0);

        var whereEqual = true;

        var inCollectionDepth = 0;

        var e1 = new ObjectEvaluator(p1);
        var e2 = new ObjectEvaluator(p2);

        e1.ScanNodes((n, k, v, t) =>
        {
            if (n.IsCollection)
            {
                whereEqual &= ContainSameItems(v, e2.Read(k), n.ElementType, ignoreId, fullTree);

                inCollectionDepth++;
            }
            else if (inCollectionDepth == 0 && n.IsLeaf)
            {
                if (!ignoreId || !IsId(k.ToString()))
                {
                    whereEqual &= AreEqualObjects(v, e2.Read(k));
                }
            }
        }, (n, k, v, t) =>
        {
            if (n.IsCollection)
            {
                inCollectionDepth--;
            }
        }, false);


        return whereEqual;
    }

    private static bool ContainSameObjects(List<object> l1, List<object> l2)
    {
        if (l1.Count != l2.Count) return false;

        foreach (var item in l1)
        {
            if (l2.All(l2I => !AreEqualObjects(item, l2I))) return false;
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