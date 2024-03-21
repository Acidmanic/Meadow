using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
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

    internal readonly List<InclusionRecord> _inclusions = new List<InclusionRecord>();

    public Type Type => typeof(TModel);

    public View()
    {
        /*
         select * from Plants;


select * from Plants left join PlantTypes on Plants.TypeId = PlantTypes.Id;


select * from Plants left join PlantTypes on Plants.TypeId = PlantTypes.Id;

select * from Plants 
    left join 
        (select * from PlantTypes where TypeName like 'Vegetable') as PT
        on Plants.TypeId = PT.Id;
         
          
         */
    }

    private sealed record JoinPoint(AccessNode IncludedNode, AccessNode Pointer, AccessNode PointedAt);

    private sealed record JoinNames(string IncludedTableName, string IncludedJoinAlias, string Pointer,
        string PointedAt);

    public string Script(MeadowConfiguration configuration)
    {
        var mainConventions = configuration.GetNameConvention(Type);

        MarkInclusions();

        var script = $"SELECT * FROM {mainConventions.TableName}";

        var fullTreeMap = configuration.GetFullTreeMap<TModel>();


        foreach (var inclusion in _inclusions)
        {
            var includedNode = fullTreeMap.AddressKeyNodeMap.NodeByKey(inclusion.IncludedField);

            var joinPoints = GetJoinPoints(includedNode);

            var joinNames = GetJoinNames(joinPoints, configuration);
            
            var where = inclusion.Conditions.Any() ? " where " : "";

            foreach (var condition in inclusion.Conditions)
            {
                var conSrcConventions = configuration.GetNameConvention(condition.SourceModelType);

                var target = "";

                if (condition.TargetValue.IsConstant)
                {
                    target = condition.TargetValue.Value;
                }
                else
                {
                    var conTarConventions = configuration.GetNameConvention(condition.TargetValue.TargetModelType!);

                    target = $"{conTarConventions.TableName}.{condition.TargetValue.FieldKey.Headless()}";
                }

                where +=
                    $" {conSrcConventions.TableName}.{condition.SourceField.Headless()} {condition.Operator} {target}";
            }

            var join = GetJoinTerm(joinPoints, where, joinNames);

            script += "\n" + join;
        }

        return script;
    }

    private JoinNames GetJoinNames(JoinPoint joinPoints, MeadowConfiguration configuration)
    {
        var pointerConv = configuration.GetNameConvention(joinPoints.Pointer.Type);

        var pointedAtConv = configuration.GetNameConvention(joinPoints.PointedAt.Type);

        if (joinPoints.IncludedNode == joinPoints.Pointer)
        {
            return new JoinNames(pointerConv.TableName, pointerConv.JoinedAliasName, pointerConv.JoinedAliasName,
                pointedAtConv.TableName);
        }
        else
        {
            return new JoinNames(pointedAtConv.TableName, pointedAtConv.JoinedAliasName, pointerConv.TableName,
                pointedAtConv.JoinedAliasName);
        }
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
            t => inclusionRecord.Conditions.Last().TargetValue = t);
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
            t => inclusionRecord.Conditions.Last().TargetValue = t);
    }


    protected virtual string QuotTableNames(string tableName)
    {
        return tableName;
    }

    // IFieldInclusionMarker<TModel> this<TProperty>[](
    //     Expression<Func<TModel, TProperty>> select,
    //     Action<QuerySource<TProperty>> where);
}