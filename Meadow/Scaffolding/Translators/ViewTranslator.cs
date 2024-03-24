using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Attributes;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Inclusion.Enums;
using Meadow.Inclusion.Fluent;
using Meadow.RelationalStandardMapping;

namespace Meadow.Scaffolding.Translators;

public class ViewTranslator
{
    
    private class Context
    {
        public NameConvention ModelTypeConventions;

        public Type ModelType;

        public ObjectEvaluator ModelEvaluator;
        
        public  Dictionary<FieldKey, NameConvention> ConventionsByFieldKey;

        public  FieldKey ModelFieldKey;
        
        public  List<FieldKey> ExtraJoins;
        
        public  List<FieldKey> AllJoins;

        public List<InclusionRecord> Inclusions;

        public FullTreeMap FullTreeMap;

        public MeadowConfiguration configurations;

        public ISqlLanguageTranslator Translator;
    }
    
    
    private sealed record JoinPoint(AccessNode IncludedNode, AccessNode Pointer, AccessNode PointedAt);

    private sealed record JoinNames(string IncludedTableName, string IncludedJoinAlias, string Pointer,
        string PointedAt);

    
    public string Script(Type modelType, MeadowConfiguration configuration, List<InclusionRecord> inclusions, ISqlLanguageTranslator translator)
    {
        //MarkInclusions();

        var context = PostProcessIndexes(modelType,configuration,inclusions, translator);
        
        var parametersTable = GetParametersTable(context);

        var script = $"SELECT {parametersTable} FROM {context.ModelTypeConventions.TableName}";

        foreach (var extraJoin in context.ExtraJoins)
        {
            var joinNode = context.FullTreeMap.AddressKeyNodeMap.NodeByKey(extraJoin);

            var joinPoints = GetJoinPoints(joinNode);

            var joinNames = GetJoinNames(joinPoints, context);

            var join = GetJoinTerm(joinPoints, "", joinNames,translator.QuotNames);

            script += "\n" + join;
        }

        foreach (var inclusion in inclusions)
        {
            var includedNode = context.FullTreeMap.AddressKeyNodeMap.NodeByKey(inclusion.IncludedField);

            var joinPoints = GetJoinPoints(includedNode);

            var joinNames = GetJoinNames(joinPoints, context);

            var where = inclusion.Conditions.Any() ? " WHERE " : "";

            var relation = BooleanRelation.None;

            foreach (var condition in inclusion.Conditions)
            {
                var conSrcConventions = configuration.GetNameConvention(condition.SourceModelType);

                var target = "";

                if (condition.Target.TargetType == TargetTypes.Constant)
                {
                    target = condition.Target.Value;
                }
                else if (condition.Target.TargetType == TargetTypes.Field)
                {
                    var conTarConventions = configuration.GetNameConvention(condition.Target.TargetModelType!);

                    target = $"{conTarConventions.TableName}.{condition.Target.FieldKey.Headless()}";
                }
                else if (condition.Target.TargetType == TargetTypes.Parameter)
                {
                    var parameterName = condition.Target.FieldKey!.TerminalSegment().Name;

                    target = $"{parameterName}";
                }

                var comparison = translator.ComparisonOperator(condition.Operator, condition.GetSourceType(),
                    condition.GetTargetType());

                var relationString = relation == BooleanRelation.None ? "" : " " + translator.RelationString(relation);

                where +=
                    $"{relationString} {conSrcConventions.TableName}.{condition.SourceField.Headless()} {comparison} {target}";

                relation = condition.NextRelation;
            }

            var join = GetJoinTerm(joinPoints, where, joinNames,translator.QuotNames);

            script += "\n" + join;
        }

        return script;
    }
    
    private Context PostProcessIndexes(Type modelType, MeadowConfiguration configuration, List<InclusionRecord> inclusions,ISqlLanguageTranslator translator)
    {

        var context = new Context()
        {
            ModelType = modelType,
            ModelTypeConventions =configuration.GetNameConvention(modelType) ,
            ModelEvaluator = new ObjectEvaluator(modelType),
            AllJoins = new List<FieldKey>(),
            ExtraJoins = new List<FieldKey>(),
            ConventionsByFieldKey = new Dictionary<FieldKey, NameConvention>(),
            Inclusions = inclusions ,
            FullTreeMap = configuration.GetFullTreeMap(modelType),
            Translator = translator,
            configurations = configuration,
        };

        context.ModelFieldKey = context.ModelEvaluator.Map.FieldKeyByNode(context.ModelEvaluator.RootNode);
        
        // Translate Inclusion keys
        foreach (var inclusion in inclusions)
        {
            inclusion.IncludedField = context.ModelEvaluator.Map.FieldKeyByAddress(inclusion.IncludedField.ToString());
        }

        ExtractJoins(context);

        context.ConventionsByFieldKey.Clear();

        foreach (var inclusion in inclusions)
        {
            context.ConventionsByFieldKey.Add(inclusion.IncludedField, configuration.GetNameConvention(inclusion.Type));
        }

        foreach (var extraJoin in context.ExtraJoins)
        {
            context.ConventionsByFieldKey.Add(extraJoin,
                configuration.GetNameConvention(context.ModelEvaluator.Map.NodeByKey(extraJoin).Type));
        }

        context.ConventionsByFieldKey.Add(context.ModelFieldKey, context.ModelTypeConventions);

        return context;
    }

    private void ExtractJoins(Context context)
    {
        context.ExtraJoins.Clear();
        context.AllJoins.Clear();

        foreach (var inclusion in context.Inclusions)
        {
            var currentKey = new FieldKey(inclusion.IncludedField);

            while (currentKey.Count > 1)
            {
                var currentNode = context.ModelEvaluator.Map.NodeByKey(currentKey);

                if (!currentNode.IsCollection)
                {
                    if (!context.ExtraJoins.Contains(currentKey) && !context.Inclusions.Any(i => i.IncludedField.Equals(currentKey)))
                    {
                        context.ExtraJoins.Add(currentKey);
                    }

                    if (!context.AllJoins.Contains(currentKey))
                    {
                        context.AllJoins.Add(currentKey);
                    }
                }

                currentKey = currentKey.UpLevel();
            }
        }
    }
    
    private class MappedKeyColumnPairComparer : IComparer<KeyValuePair<string, FieldKey>>
    {
        public int Compare(KeyValuePair<string, FieldKey> x, KeyValuePair<string, FieldKey> y)
        {
            return x.Value.Count - y.Value.Count;
        }
    }

    
    private string GetParametersTable(Context context)
    {
        var relationalMap = IncludedColumns(context);

        relationalMap.Sort(new MappedKeyColumnPairComparer());

        var parameterTable = "";
        var tab = "        ";
        var sep = "";
        foreach (var columnKey in relationalMap)
        {
            var fullTreeAlias = columnKey.Key;
            var key = columnKey.Value;
            var node = context.FullTreeMap.AddressKeyNodeMap.NodeByKey(key);
            var ownerType = node.Parent.Type;
            var ownerConventions = context.configurations.GetNameConvention(ownerType);
            var tableName = context.ModelType == ownerType ? ownerConventions.TableName : ownerConventions.JoinedAliasName;
            var originalColumnName = context.Translator.QuotNames(tableName) + "." + context.Translator.QuotNames(node.Name);

            parameterTable += sep + tab + originalColumnName + tab + $"{context.Translator.TableAliasQuot}{fullTreeAlias}{context.Translator.TableAliasQuot}";
            sep = ",\n";
        }

        return parameterTable.Trim();
    }
    
    
    private List<KeyValuePair<string, FieldKey>> IncludedColumns(Context context)
    {
        var columns = new List<KeyValuePair<string, FieldKey>>();

        var inclusions = context.Inclusions.Select(i => i.IncludedField).ToList();

        var firstSegment = context.ModelEvaluator.RootNode.Name;

        foreach (var columnKey in context.FullTreeMap.RelationalMap)
        {
            if (inclusions.Any(i => columnKey.Value.StartsWith(i) && columnKey.Value.Count == i.Count + 1))
            {
                columns.Add(columnKey);
            }
            else if (columnKey.Value.Count == 2 && columnKey.Value[0].Name == firstSegment)
            {
                columns.Add(columnKey);
            }
        }

        return columns;
    }


    private string GetJoinName(AccessNode node, Context context)
    {
        var key = context.ModelEvaluator.Map.FieldKeyByAddress(node.GetFullName());

        if (context.AllJoins.Contains(key))
        {
            return context.ConventionsByFieldKey[key].JoinedAliasName;
        }

        return context.ConventionsByFieldKey[key].TableName;
    }

    private JoinNames GetJoinNames(JoinPoint joinPoints, Context context)
    {
        var includedConventions =
            context.ConventionsByFieldKey[context.ModelEvaluator.Map.FieldKeyByAddress(joinPoints.IncludedNode.GetFullName())];

        return new JoinNames(includedConventions.TableName, includedConventions.JoinedAliasName,
            GetJoinName(joinPoints.Pointer, context), GetJoinName(joinPoints.PointedAt, context));
    }

    private string GetJoinTerm(JoinPoint join, string where, JoinNames joinAlias,Func<string, string> q)
    {
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

}