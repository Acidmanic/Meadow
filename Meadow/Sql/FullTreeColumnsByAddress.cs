using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Results;
using Meadow.Contracts;
using Meadow.Extensions;

namespace Meadow.Sql;

public class FullTreeColumnsByAddress<T> : FullTreeColumnsByAddress
{
    public FullTreeColumnsByAddress(char fieldNameDelimiter) : base(typeof(T), fieldNameDelimiter)
    {
    }
}

public class FullTreeColumnsByAddress
{
    private readonly Dictionary<string, string> _columnsByAddress = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _columnsByHeadlessAddress = new Dictionary<string, string>();


    private NameConvention NameConvention { get; }

    public FullTreeColumnsByAddress(Type type, char fieldNameDelimiter)
    {
        var evaluator = new ObjectEvaluator(type);

        NameConvention = new NameConvention(type);

        var counts = CountNodeNameRepetitions(evaluator.Map.Nodes.Where(n => n.IsLeaf));

        foreach (var nodeCount in counts)
        {
            var key = evaluator.Map.FieldKeyByNode(nodeCount.Key);

            var column = GetDatabaseFieldName(evaluator.Map, key, nodeCount.Value > 1, fieldNameDelimiter);

            var headless = key.Headless().ToString();

            _columnsByAddress.Add(key.ToString().ToLower(), column);

            _columnsByHeadlessAddress.Add(headless.ToLower(), column);
        }
    }

    public Result<string> GetColumnName(string headlessAddress)
    {
        var key = headlessAddress.ToLower();

        if (_columnsByHeadlessAddress.ContainsKey(key))
        {
            return new Result<string>(true, _columnsByHeadlessAddress[key]);
        }

        return new Result<string>().FailAndDefaultValue();
    }

    public Result<string> GetColumnNameByFullAddress(string address)
    {
        var key = address.ToLower();

        if (_columnsByAddress.ContainsKey(key))
        {
            return new Result<string>(true, _columnsByAddress[key]);
        }

        return new Result<string>().FailAndDefaultValue();
    }

    private string GetDatabaseFieldName(AddressKeyNodeMap map, FieldKey key, bool fullAddress, char delimiter)
    {
        if (!fullAddress)
        {
            return key.TerminalSegment().Name;
        }

        var column = "";

        for (int i = 1; i < key.Count; i++)
        {
            var dataOwnerKey = key.Subkey(0, i);

            var ownerNode = map.NodeByKey(dataOwnerKey);

            if (!ownerNode.IsCollection)
            {
                var type = ownerNode.Type;

                var tableName = NameConvention.TableNameProvider.GetNameForOwnerType(type);

                column += tableName + delimiter;
            }
        }

        column += key.TerminalSegment().Name;

        return column;
    }

    private Dictionary<AccessNode, int> CountNodeNameRepetitions(IEnumerable<AccessNode> nodes)
    {
        var nameCounts = new Dictionary<string, int>();

        foreach (var node in nodes)
        {
            var count = 0;
            var name = node.Name.ToLower();

            if (nameCounts.ContainsKey(name))
            {
                count = nameCounts[name];

                nameCounts.Remove(name);
            }

            nameCounts.Add(name, count + 1);
        }

        var nodeNameRepetitionsCount = new Dictionary<AccessNode, int>();

        foreach (var node in nodes)
        {
            var name = node.Name.ToLower();

            var count = nameCounts[name];

            nodeNameRepetitionsCount.Add(node, count);
        }

        return nodeNameRepetitionsCount;
    }
}