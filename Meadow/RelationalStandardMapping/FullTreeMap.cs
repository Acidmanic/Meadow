using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Results;
using Meadow.Contracts;
using Meadow.Extensions;

namespace Meadow.RelationalStandardMapping;

public class FullTreeMap<T> : FullTreeMap
{
    public FullTreeMap(char fieldNameDelimiter, IDataOwnerNameProvider dataOwnerNameProvider)
        : base(typeof(T), fieldNameDelimiter, dataOwnerNameProvider)
    {
    }
}

public class FullTreeMap
{
    private readonly Dictionary<string, string> _columnsByAddress = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _columnsByHeadlessAddress = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _addressesByColumnName = new Dictionary<string, string>();
    private readonly Dictionary<string, FieldKey> _keysByColumnName = new Dictionary<string, FieldKey>();
    private readonly Dictionary<string, FieldKey> _keysByCapitalColumnName = new Dictionary<string, FieldKey>();


    public NameConvention NameConvention { get; }
    
    public AddressKeyNodeMap AddressKeyNodeMap { get; }
    
    public IEnumerable<string> Columns => new List<string>(_columnsByAddress.Values);
    public IEnumerable<FieldKey> Keys => new List<FieldKey>(_keysByColumnName.Values);
    public IEnumerable<string> Addresses => new List<string>(_addressesByColumnName.Values);

    public Dictionary<string, FieldKey> RelationalMap => new Dictionary<string, FieldKey>(_keysByCapitalColumnName);
    
    public ObjectEvaluator Evaluator { get; }

    public FullTreeMap(Type type, char fieldNameDelimiter, IDataOwnerNameProvider dataOwnerNameProvider)
    {
        var evaluator = new ObjectEvaluator(type);

        Evaluator = evaluator;
        
        AddressKeyNodeMap = evaluator.Map;
        
        NameConvention = new NameConvention(type, dataOwnerNameProvider);

        var counts = CountNodeNameRepetitions(evaluator.Map.Nodes.Where(n => n.IsLeaf));

        foreach (var nodeCount in counts)
        {
            var key = evaluator.Map.FieldKeyByNode(nodeCount.Key);

            var column = GetDatabaseFieldName(evaluator.Map, key, nodeCount.Value > 1, fieldNameDelimiter);

            var headless = key.Headless().ToString();

            _columnsByAddress.Add(key.ToString().ToLower(), column);

            _columnsByHeadlessAddress.Add(headless.ToLower(), column);

            _addressesByColumnName.Add(column.ToLower(), key.ToString());

            _keysByColumnName.Add(column.ToLower(), key);

            _keysByCapitalColumnName.Add(column, key);
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

    public Result<string> GetAddressByColumnName(string columnName)
    {
        var key = columnName.ToLower();

        if (_addressesByColumnName.ContainsKey(key))
        {
            return new Result<string>(true, _addressesByColumnName[key]);
        }

        return new Result<string>().FailAndDefaultValue();
    }

    public Result<FieldKey> GetKeyByColumnName(string columnName)
    {
        var key = columnName.ToLower();

        if (_keysByColumnName.ContainsKey(key))
        {
            return new Result<FieldKey>(true, _keysByColumnName[key]);
        }

        return new Result<FieldKey>().FailAndDefaultValue();
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