using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Attributes;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Inclusion.Fluent;
using Meadow.Inclusion.Fluent.Markers;
using Meadow.RelationalStandardMapping;

namespace Meadow.Inclusion;

public abstract class View<TModel>
{
    protected abstract void MarkInclusions();

    private readonly List<InclusionRecord> _inclusions;
    
    private readonly Dictionary<FieldKey, NameConvention> _conventionsByFieldKey;

    private readonly ObjectEvaluator _modelEvaluator ;

    private readonly FieldKey _modelFieldKey ;

    private NameConvention _modelTypeConventions = new NameConvention(typeof(TModel));
    
    private readonly List<FieldKey> _extraJoins;
    private readonly List<FieldKey> _allJoins;

    public Type ModelType => typeof(TModel);
    
    
    private void PostProcessIndexes(MeadowConfiguration configuration)
    {
        // Translate Inclusion keys
        foreach (var inclusion in _inclusions)
        {
            inclusion.IncludedField = _modelEvaluator.Map.FieldKeyByAddress(inclusion.IncludedField.ToString());
        }
        
        ExtractJoins();
        
        _modelTypeConventions = configuration.GetNameConvention(ModelType);
        
        _conventionsByFieldKey.Clear();
        
        foreach (var inclusion in _inclusions)
        {
            _conventionsByFieldKey.Add(inclusion.IncludedField,configuration.GetNameConvention(inclusion.Type)); 
        }

        foreach (var extraJoin in _extraJoins)
        {
            _conventionsByFieldKey.Add(extraJoin,configuration.GetNameConvention(_modelEvaluator.Map.NodeByKey(extraJoin).Type)); 
        }
        _conventionsByFieldKey.Add(_modelFieldKey,_modelTypeConventions);

        
    }

    private void ExtractJoins()
    {
        _extraJoins.Clear();
        _allJoins.Clear();
        
        foreach (var inclusion in _inclusions)
        {
            var currentKey = new FieldKey(inclusion.IncludedField);
        
            while (currentKey.Count >1)
            {
                var currentNode = _modelEvaluator.Map.NodeByKey(currentKey);

                if (!currentNode.IsCollection)
                {
                    if (!_extraJoins.Contains(currentKey) && ! _inclusions.Any(i => i.IncludedField.Equals(currentKey)))
                    {
                        _extraJoins.Add(currentKey);
                    }

                    if (!_allJoins.Contains(currentKey))
                    {
                        _allJoins.Add(currentKey);
                    }
                }

                currentKey = currentKey.UpLevel();
            }
        }
    }
    
    public View()
    {
        _inclusions = new List<InclusionRecord>();
        _conventionsByFieldKey = new Dictionary<FieldKey, NameConvention>();
        _modelEvaluator = new ObjectEvaluator(typeof(TModel));
        _modelFieldKey = _modelEvaluator.Map.FieldKeyByNode(_modelEvaluator.RootNode);
        _extraJoins = new List<FieldKey>();
        _allJoins = new List<FieldKey>();
    }

    private sealed record JoinPoint(AccessNode IncludedNode, AccessNode Pointer, AccessNode PointedAt);

    private sealed record JoinNames(string IncludedTableName, string IncludedJoinAlias, string Pointer,
        string PointedAt);

    public string Script(MeadowConfiguration configuration)
    {
        
        MarkInclusions();

        PostProcessIndexes(configuration);
        
       
        var fullTreeMap = configuration.GetFullTreeMap<TModel>();

        var parametersTable = GetParametersTable(QuotTableNames, fullTreeMap, configuration);
        
        var script = $"SELECT {parametersTable} FROM {_modelTypeConventions.TableName}";

        foreach (var extraJoin in _extraJoins)
        {
            var joinNode = fullTreeMap.AddressKeyNodeMap.NodeByKey(extraJoin);

            var joinPoints = GetJoinPoints(joinNode);

            var joinNames = GetJoinNames(joinPoints, configuration);
            
            var join = GetJoinTerm(joinPoints, "", joinNames);

            script += "\n" + join;
        }
        
        foreach (var inclusion in _inclusions)
        {
            var includedNode = fullTreeMap.AddressKeyNodeMap.NodeByKey(inclusion.IncludedField);

            var joinPoints = GetJoinPoints(includedNode);

            var joinNames = GetJoinNames(joinPoints, configuration);
            
            var where = inclusion.Conditions.Any() ? " WHERE " : "";

            foreach (var condition in inclusion.Conditions)
            {
                var conSrcConventions = configuration.GetNameConvention(condition.SourceModelType);

                var target = "";

                if (condition.Target.IsConstant)
                {
                    target = condition.Target.Value;
                }
                else
                {
                    var conTarConventions = configuration.GetNameConvention(condition.Target.TargetModelType!);

                    target = $"{conTarConventions.TableName}.{condition.Target.FieldKey.Headless()}";
                }

                var comparison = TranslateComparisonOperators(condition.Operator, condition.GetSourceType(), condition.GetTargetType());
                
                where +=
                    $" {conSrcConventions.TableName}.{condition.SourceField.Headless()} {comparison} {target}";
            }

            var join = GetJoinTerm(joinPoints, where, joinNames);

            script += "\n" + join;
        }

       

        return script;
    }

   


    private bool StartsWith(FieldKey start, FieldKey key)
    {
        if (key.Count < start.Count)
        {
            return false;
        }
        
        for (int i = 0; i < start.Count; i++)
        {
            if (start[i].Name != key[i].Name)
            {
                return false;
            }
        }

        return true;
    }
    

    private List<KeyValuePair<string,FieldKey>> IncludedColumns(FullTreeMap fullTreeMap)
    {
        var columns = new List<KeyValuePair<string,FieldKey>>();

        // var inclusions = _inclusions.Select(i =>
        //     i.IncludedField.TerminalSegment().Indexed ? i.IncludedField.UpLevel() : i.IncludedField).ToList();
        //
        
        var inclusions = _inclusions.Select(i => i.IncludedField).ToList();
        
        var firstSegment = new ObjectEvaluator(typeof(TModel)).RootNode.Name;
        
        foreach (var columnKey in fullTreeMap.RelationalMap)
        {
            if (inclusions.Any(i => StartsWith(i,columnKey.Value) && columnKey.Value.Count == i.Count+1))
            {
                columns.Add(columnKey);
            }else if (columnKey.Value.Count == 2 && columnKey.Value[0].Name == firstSegment)
            {
                columns.Add(columnKey);
            }
        }

        return columns;
    }


    private string GetParametersTable(Func<string, string> q, FullTreeMap fullTreeMap, MeadowConfiguration configuration)
    {
        var relationalMap = IncludedColumns(fullTreeMap);
        
        var parameterTable = "";
        var tab = "        ";
        var sep = "";
        foreach (var columnKey in relationalMap)
        {
            var fullTreeAlias = columnKey.Key;
            var key = columnKey.Value;
            var node = fullTreeMap.AddressKeyNodeMap.NodeByKey(key);
            var ownerType = node.Parent.Type;
            var ownerConventions = configuration.GetNameConvention(ownerType);
            var tableName = ModelType==ownerType? ownerConventions.TableName: ownerConventions.JoinedAliasName;
            var originalColumnName = q(tableName) + "." + q(node.Name);

            parameterTable += sep + tab + originalColumnName + tab + $"{AliasQuote}{fullTreeAlias}{AliasQuote}";
            sep = ",\n";
        }

        return parameterTable.Trim();
    }

    private string GetJoinName(AccessNode node, MeadowConfiguration configuration)
    {
        var key = _modelEvaluator.Map.FieldKeyByAddress(node.GetFullName());

        if (_allJoins.Contains(key))
        {
            return _conventionsByFieldKey[key].JoinedAliasName;
        }

        return _conventionsByFieldKey[key].TableName;
    }

    private JoinNames GetJoinNames(JoinPoint joinPoints, MeadowConfiguration configuration)
    {
        // var pointerConv = configuration.GetNameConvention(joinPoints.Pointer.Type);
        //
        // var pointedAtConv = configuration.GetNameConvention(joinPoints.PointedAt.Type);
        //
        //
        // if (joinPoints.IncludedNode == joinPoints.Pointer)
        // {
        //     return new JoinNames(pointerConv.TableName, pointerConv.JoinedAliasName, pointerConv.JoinedAliasName,
        //         pointedAtConv.TableName);
        // }
        // else
        // {
        //     return new JoinNames(pointedAtConv.TableName, pointedAtConv.JoinedAliasName, pointerConv.TableName,
        //         pointedAtConv.JoinedAliasName);
        // }

        var includedConventions = _conventionsByFieldKey[_modelEvaluator.Map.FieldKeyByAddress(joinPoints.IncludedNode.GetFullName())];

        // if (_conventionsByFieldKey.ContainsKey(_modelEvaluator.Map.FieldKeyByNode(joinPoints.Pointer)))
        // {
        //     
        // }

        return new JoinNames(includedConventions.TableName, includedConventions.JoinedAliasName,
            GetJoinName(joinPoints.Pointer, configuration), GetJoinName(joinPoints.PointedAt, configuration));
    }

    private string GetJoinTerm(JoinPoint join, string where, JoinNames joinAlias)
    {
        Func<string, string> q = QuotTableNames;
        var pointerIdFieldName = GetReferencedIdFieldName(join.PointedAt);
        var pointedAtIdField = TypeIdentity.FindIdentityLeaf(join.PointedAt.Type).Name;

        return
            $"LEFT JOIN (SELECT * FROM {q(joinAlias.IncludedTableName)}{where}) AS {joinAlias.IncludedJoinAlias} ON {q(joinAlias.Pointer)}.{q(pointerIdFieldName)} =" +
            $" {q(joinAlias.PointedAt)}.{q(pointedAtIdField)}";
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


    private JoinPoint GetJoinPoints(AccessNode includedNode)
    {
        if (includedNode.IsCollectable)
        {
            // N -> 1 | GrandFather
            return new JoinPoint(includedNode, includedNode, includedNode.Parent.Parent);
        }
        else
        {
            //The parent 
            return new JoinPoint(includedNode, includedNode.Parent, includedNode);
        }
    }

    

    protected IQuerySource<TModel, TProperty> Include<TProperty>(
        Expression<Func<TModel, List<TProperty>>> select)
    {
        var includedField = MemberOwnerUtilities.GetKey(select);
        
        
        var inclusionRecord = new InclusionRecord
        {
            Conditions = new List<InclusionCondition>(),
            IncludedField = includedField,
            Type = typeof(TProperty)
        };

        _inclusions.Add(inclusionRecord);

        return new FieldAddressQuerySource<TModel, TProperty>(
            sfk =>
            {
                var condition = new InclusionCondition
                {
                    SourceModelType = typeof(TProperty),
                    SourceField = sfk
                };
                inclusionRecord.Conditions.Add(condition);
            }, o => inclusionRecord.Conditions.Last().Operator = o,
            t => inclusionRecord.Conditions.Last().Target = t);
    }

    protected IQuerySource<TModel, TProperty> Include<TProperty>(
        Expression<Func<TModel, TProperty>> select)
    {
        var inclusionRecord = new InclusionRecord
        {
            Conditions = new List<InclusionCondition>(),
            IncludedField = MemberOwnerUtilities.GetKey(select),
            Type = typeof(TProperty)
        };

        _inclusions.Add(inclusionRecord);

        return new FieldAddressQuerySource<TModel, TProperty>(
            sfk =>
            {
                var condition = new InclusionCondition
                {
                    SourceModelType = typeof(TProperty),
                    SourceField = sfk
                };
                inclusionRecord.Conditions.Add(condition);
            }, o => inclusionRecord.Conditions.Last().Operator = o,
            t => inclusionRecord.Conditions.Last().Target = t);
    }


    protected virtual string QuotTableNames(string tableName)
    {
        return tableName;
    }

    protected virtual string TranslateComparisonOperators(Operators o,Type sourceType, Type targetType)
    {
        var stringType = typeof(string);

        var isString = sourceType == stringType || targetType == stringType;

        var equality = isString ? "like" : "=";
        
        var inEquality = isString ? "NOT LIKE" : "!=";
        
        if (o == Operators.IsEqualTo)
        {
            return equality;
        }else if (o == Operators.IsNotEqualTo)
        {
            return inEquality;
        }else if (o == Operators.IsGreaterThan)
        {
            return ">";
        }else if (o == Operators.IsSmallerThan)
        {
            return "<";
        }else if (o == Operators.IsGreaterOrEqualTo)
        {
            return ">=";
        }else if (o == Operators.IsSmallerOrEqualTo)
        {
            return "<=";
        }

        return "=";
    }
    
    protected virtual string AliasQuote => "'";
    
    // IFieldInclusionMarker<TModel> this<TProperty>[](
    //     Expression<Func<TModel, TProperty>> select,
    //     Action<QuerySource<TProperty>> where);
}