using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Results;
using Meadow.Attributes;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.Models;

namespace Meadow.Sql;

public class FullTreeTranslation
{
    private readonly FullTreeMap _fullTreeMap;
    private readonly MeadowConfiguration _meadowConfiguration;
    private readonly ProcessedType _processedType;
    private readonly ISqlTranslator _sqlTranslator;
    private readonly QuoterSet _q;
    public FullTreeTranslation(MeadowConfiguration meadowConfiguration, ProcessedType processedType, ISqlTranslator sqlTranslator)
    {
        _meadowConfiguration = meadowConfiguration;
        _fullTreeMap = _meadowConfiguration.GetFullTreeMap(processedType.NameConvention.EntityType);
        _processedType = processedType;
        _sqlTranslator = sqlTranslator;
        _processedType.NameConvention.EntityType.GetAlteredOrOriginal();
        _q = _sqlTranslator.GetQuoters();
    }

    public string GetFullTreeColumnNameByAddress(string address)
    {
        return _fullTreeMap.GetColumnName(FieldKey.Parse(address).Headless().ToString());
    }
    
    public string GetFullTreeAliasedParameters()
    {

        var relationalMap = _fullTreeMap.RelationalMap;
        var parameterTable = "";
        var tab = "        ";
        var sep = "";
        foreach (var columnKey in relationalMap)
        {
            var fullTreeAlias = columnKey.Key;
            var key = columnKey.Value;
            var node = _fullTreeMap.AddressKeyNodeMap.NodeByKey(key);
            var ownerType = node.Parent.Type;
            var tableName = _processedType.NameConvention.TableNameProvider.GetNameForOwnerType(ownerType);
            var originalColumnName = _q.QuoteTableName(tableName) + "." + _q.QuoteColumnName(node.Name);

            parameterTable += sep + tab + originalColumnName + tab + $"{_sqlTranslator.ColumnNameAliasQuote}{fullTreeAlias}{_sqlTranslator.ColumnNameAliasQuote}";
            sep = ",\n";
        }

        return parameterTable.Trim();
    }
    
    public string GetInnerJoins()
    {
        
        var joinNodes = GetJoinNodes();

        var joins = "";

        foreach (var joinNode in joinNodes)
        {
            AccessNode pointerNode, nodePointedAt;
            if (joinNode.IsCollectable)
            {
                // N -> 1
                pointerNode = joinNode;
                // The grandFather
                nodePointedAt = joinNode.Parent.Parent;
            }
            else
            {
                //The parent 
                pointerNode = joinNode.Parent;
                nodePointedAt = joinNode;
            }

            string joinTerm = GetJoinTerm(joinNode, pointerNode, nodePointedAt);
            joins += "\n        " + joinTerm;
        }

        return joins;
    }
    
    private IEnumerable<AccessNode> GetJoinNodes()
    {
        var ev = _fullTreeMap.Evaluator;
        var rootNode = ev.RootNode;

        var joinNodes = _fullTreeMap.AddressKeyNodeMap.Nodes
            .Where(n => !n.IsLeaf && !n.IsCollection && n != rootNode);

        return joinNodes;
    }

    
    private string GetReferencedIdFieldName(AccessNode node)
    {
        var foundAttribute = node.Type.GetCustomAttributes(true).FirstOrDefault(a => a is OneToMany);

        if (foundAttribute is OneToMany nToOneAtt)
        {
            return nToOneAtt.ReferenceFieldName;
        }

        return node.Name + "Id";
    }

    private FilterQuery GetRegisteredFilter(Type type)
    {
            
        if (_meadowConfiguration.Filters.ContainsKey(type))
        {
            return _meadowConfiguration.Filters[type];
        }

        return new FilterQuery();
    }
    
    private string GetJoinTerm(AccessNode joinNode, AccessNode pointerNode, AccessNode nodePointedAt)
    {
        var joinTableName = _processedType.NameConvention.TableNameProvider.GetNameForOwnerType(joinNode.Type);
        var pointerTableName = _processedType.NameConvention.TableNameProvider.GetNameForOwnerType(pointerNode.Type);
        var pointerIdFieldName = GetReferencedIdFieldName(nodePointedAt);
        var pointedAtTableName =
            _processedType.NameConvention.TableNameProvider.GetNameForOwnerType(nodePointedAt.Type);
        var pointedAtIdField = TypeIdentity.FindIdentityLeaf(nodePointedAt.Type).Name;

        var queryFilter = GetRegisteredFilter(joinNode.Type);
        //q(joinTableName)
        var source = GetAlternatedSelectSource(queryFilter, joinTableName);
            
        return
            $"LEFT JOIN {source} ON {_q.QuoteTableName(pointerTableName)}.{_q.QuoteColumnName(pointerIdFieldName)} =" +
            $" {_q.QuoteTableName(pointedAtTableName)}.{_q.QuoteColumnName(pointedAtIdField)}";
    }
    
    
    private string GetAlternatedSelectSource(FilterQuery queryFilter, string name)
    {
        var whereClause = GetFiltersWhereClause(ColumnNameTranslation.FullTree,queryFilter);

        if (whereClause)
        {
            return $"(SELECT * FROM {_q.QuoteTableName(name)} WHERE {whereClause.Value}) AS {_q.QuoteTableName(name)}";
        }

        return _q.QuoteTableName(name);
    }


    private Result<string> GetFiltersWhereClause(ColumnNameTranslation translation, FilterQuery queryFilter)
    {
        var filterItems = queryFilter.Items();

        var count = filterItems?.Count ?? 0;

        if (count == 0)
        {
            return new Result<string>().FailAndDefaultValue();
        }

        var translatedQuery = _sqlTranslator.TranslateFilterQueryToDbExpression(queryFilter, translation);

        return new Result<string>(true, translatedQuery);
    }
}