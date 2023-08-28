using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Contracts;
using Meadow.Sql;

namespace Meadow.RelationalStandardMapping;

public class FlatRelationalToStandardMapper : IRelationalIdentifierToStandardFieldMapper
{
    public FlatRelationalToStandardMapper(IDataOwnerNameProvider dataOwnerNameProvider)
    {
        DataOwnerNameProvider = dataOwnerNameProvider;
    }

    public char DatabaseFieldNameDelimiter { get; set; } = '_';

    public IDataOwnerNameProvider DataOwnerNameProvider { get; }

    public Dictionary<string, FieldKey> MapAddressesByIdentifier<TModel>(bool fullTree = true)
    {
        return MapAddressesByIdentifier(typeof(TModel), fullTree);
    }

    public Dictionary<string, FieldKey> MapAddressesByIdentifier(Type type, bool fullTree = true)
    {
        if (fullTree)
        {
            var map = new FullTreeMap(type, DatabaseFieldNameDelimiter, DataOwnerNameProvider)
            {
            };

            return map.RelationalMap;
        }

        return GetDirectMap(type);
    }

    public Dictionary<string, FieldKey> GetDirectMap(Type type)
    {
        var map = new Dictionary<string, FieldKey>();

        var evaluator = new ObjectEvaluator(type);

        var leaves = evaluator.RootNode.GetDirectLeaves();

        foreach (var leaf in leaves)
        {
            var key = evaluator.Map.FieldKeyByNode(leaf);

            map.Add(leaf.Name, key);
        }

        return map;
    }
}