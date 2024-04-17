using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        public Dictionary<FieldKey, NameConvention> ConventionsByFieldKey;

        public FieldKey ModelFieldKey;

        public List<FieldKey> JoinOnlyEntities;

        public List<FieldKey> RelatedEntities;

        public List<InclusionRecord> Inclusions;

        public FullTreeMap FullTreeMap;

        public MeadowConfiguration Configurations;

        public ISqlLanguageTranslator Translator;

        public Dictionary<FieldKey, bool> HasCondition;

        public Dictionary<FieldKey, bool> IsIncluded;
    }


    private sealed record JoinPoint(FieldKey IncludedKey, FieldKey Pointer, FieldKey PointedAt);

    private string CreateJoin(FieldKey entity, Context context, string where = "")
    {
        var joinPoints = GetJoinPoints(entity, context);

        var join = GetJoinTerm(joinPoints, where, context);

        return join;
    }

    public string Script(Type modelType, MeadowConfiguration configuration, List<InclusionRecord> inclusions,
        ISqlLanguageTranslator translator)
    {
        var context = PostProcessIndexes(modelType, configuration, inclusions, translator);

        var parametersTable = GetParametersTable(context);

        var script = $"SELECT {parametersTable} FROM {context.ModelTypeConventions.TableName}";

        script += "\n" + string.Join("\n", context.JoinOnlyEntities!.Select(e => CreateJoin(e, context)));

        foreach (var inclusion in inclusions)
        {
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

            script += "\n" + CreateJoin(inclusion.IncludedField, context, where);
        }

        return script;
    }

    private Context PostProcessIndexes(Type modelType, MeadowConfiguration configuration,
        List<InclusionRecord> inclusions, ISqlLanguageTranslator translator)
    {
        var context = new Context()
        {
            ModelType = modelType,
            ModelTypeConventions = configuration.GetNameConvention(modelType),
            ModelEvaluator = new ObjectEvaluator(modelType),
            RelatedEntities = new List<FieldKey>(),
            JoinOnlyEntities = new List<FieldKey>(),
            ConventionsByFieldKey = new Dictionary<FieldKey, NameConvention>(),
            Inclusions = inclusions,
            FullTreeMap = configuration.GetFullTreeMap(modelType),
            Translator = translator,
            Configurations = configuration,
            HasCondition = new Dictionary<FieldKey, bool>(),
            IsIncluded = new Dictionary<FieldKey, bool>(),
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

        foreach (var extraJoin in context.JoinOnlyEntities)
        {
            context.ConventionsByFieldKey.Add(extraJoin,
                configuration.GetNameConvention(context.ModelEvaluator.Map.NodeByKey(extraJoin).Type));
        }

        context.ConventionsByFieldKey.Add(context.ModelFieldKey, context.ModelTypeConventions);

        return context;
    }

    private void ExtractJoins(Context context)
    {
        context.JoinOnlyEntities.Clear();
        context.RelatedEntities.Clear();

        foreach (var inclusion in context.Inclusions)
        {
            var currentKey = new FieldKey(inclusion.IncludedField);

            while (currentKey.Count > 1)
            {
                var currentNode = context.ModelEvaluator.Map.NodeByKey(currentKey);

                if (!currentNode.IsCollection)
                {
                    if (!context.JoinOnlyEntities.Contains(currentKey) &&
                        !context.Inclusions.Any(i => i.IncludedField.Equals(currentKey)))
                    {
                        context.JoinOnlyEntities.Add(currentKey);
                    }

                    if (!context.RelatedEntities.Contains(currentKey))
                    {
                        context.RelatedEntities.Add(currentKey);
                    }
                }

                currentKey = currentKey.UpLevel();
            }
        }

        foreach (var entity in context.RelatedEntities)
        {
            var inclusion = context.Inclusions.FirstOrDefault(I => I.IncludedField.EqualsIgnoreIndex(entity));

            var isIncluded = inclusion != null;

            context.IsIncluded.Add(entity, isIncluded);

            bool hasCondition = inclusion?.Conditions.Count > 0;

            context.HasCondition.Add(entity, hasCondition);
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
            
            var node = context.FullTreeMap.AddressKeyNodeMap.NodeByKey(columnKey.Value);
            
            var tableName = GetSourceNameForUsage(columnKey.Value.UpLevel(), context);
            
            var originalColumnName =
                context.Translator.QuoteTableName(tableName) + "." + context.Translator.QuoteTableName(node.Name);

            parameterTable += sep + tab + originalColumnName + tab +
                              $"{context.Translator.TableAliasQuot}{fullTreeAlias}{context.Translator.TableAliasQuot}";
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


    private bool HasCondition(FieldKey key, Context context)
    {
        if (context.HasCondition.ContainsKey(key)) return context.HasCondition[key];

        return false;
    }

    private string GetSourceNameForUsage(FieldKey key, Context context)
    {
        if (HasCondition(key, context))
        {
            return context.ConventionsByFieldKey[key].JoinedAliasName;
        }

        return context.ConventionsByFieldKey[key].TableName;
    }

    private string GetJoinTerm(JoinPoint join, string where, Context context)
    {
        var includedConventions = context.ConventionsByFieldKey[join.IncludedKey];

        var q = context.Translator.QuoteTableName;

        var pointerIdFieldName = GetReferencedIdFieldName(join.PointedAt, context);

        var pointerSourceName = GetSourceNameForUsage(join.Pointer, context);

        var pointedAtSourceName = GetSourceNameForUsage(join.PointedAt, context);

        var pointedAtIdField = TypeIdentity.FindIdentityLeaf(context.ModelEvaluator.Map.NodeByKey(join.PointedAt).Type)
            .Name;
        if (HasCondition(join.IncludedKey, context))
        {
            return
                $"LEFT JOIN (SELECT * FROM {q(includedConventions.TableName)}{where}) AS " +
                $"{includedConventions.JoinedAliasName} ON {q(pointerSourceName)}.{q(pointerIdFieldName)} =" +
                $" {q(pointedAtSourceName)}.{q(pointedAtIdField)}";
        }
        else
        {
            return $"LEFT JOIN {includedConventions.TableName} ON {q(pointerSourceName)}.{q(pointerIdFieldName)} =" +
                   $" {q(pointedAtSourceName)}.{q(pointedAtIdField)}";
        }
    }

    private string GetReferencedIdFieldName(FieldKey field, Context context)
    {
        var node = context.ModelEvaluator.Map.NodeByKey(field);

        var foundAttribute = node.Type.GetCustomAttributes(true).FirstOrDefault(a => a is OneToMany);

        if (foundAttribute is OneToMany nToOneAtt)
        {
            return nToOneAtt.ReferenceFieldName;
        }

        return node.Name + "Id";
    }


    private JoinPoint GetJoinPoints(FieldKey includedKey, Context context)
    {
        var includedNode = context.ModelEvaluator.Map.NodeByKey(includedKey);

        FieldKey pointer;
        FieldKey pointed;

        if (includedNode.IsCollectable)
        {
            // N -> 1 | GrandFather
            pointer = includedKey;
            pointed = context.ModelEvaluator.Map.FieldKeyByNode(includedNode.Parent.Parent);
        }
        else
        {
            //The parent
            pointer = context.ModelEvaluator.Map.FieldKeyByNode(includedNode.Parent);
            pointed = includedKey;
        }

        return new JoinPoint(includedKey, pointer, pointed);
    }
}