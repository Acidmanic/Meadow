using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Inclusion.Fluent;
using Meadow.Inclusion.Fluent.Markers;

namespace Meadow.Inclusion;

public abstract class ViewBase<TModel>
{
    

    private readonly List<InclusionRecord> _inclusions;

  
    public ViewBase()
    {
        _inclusions = new List<InclusionRecord>();
    }


    protected IQuerySource<TParametersModel, TModel, TProperty> IncludeWithParameters<TParametersModel, TProperty>(
        FieldKey includedField)
    {
        var inclusionRecord = new InclusionRecord
        {
            Conditions = new List<InclusionCondition>(),
            IncludedField = includedField,
            Type = typeof(TProperty)
        };

        AddInclusion(inclusionRecord);

        var source = new FieldAddressQuerySource<TParametersModel, TModel, TProperty>(
            sfk =>
            {
                var condition = new InclusionCondition
                {
                    SourceModelType = typeof(TProperty),
                    SourceField = sfk
                };
                inclusionRecord.Conditions.Add(condition);
            }, o => inclusionRecord.Conditions.Last().Operator = o,
            t => inclusionRecord.Conditions.Last().Target = t,
            b => { _inclusions.Last().Conditions.Last().NextRelation = b; });

        return source;
    }

    protected IQuerySource<TModel, TProperty> IncludeWithoutParameters<TProperty>(FieldKey includedField)
    {
        var inclusionRecord = new InclusionRecord
        {
            Conditions = new List<InclusionCondition>(),
            IncludedField = includedField,
            Type = typeof(TProperty)
        };

        AddInclusion(inclusionRecord);

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
            t => inclusionRecord.Conditions.Last().Target = t,
            b => { _inclusions.Last().Conditions.Last().NextRelation = b; });
    }

    internal void AddInclusion(InclusionRecord inclusionRecord)
    {
        _inclusions.Add(inclusionRecord);
    }

    public List<InclusionRecord> Inclusions
    {
        get
        {
            _inclusions.Clear();
            
            MarkInclusions();

            return _inclusions;
        }   
    }

    protected abstract void MarkInclusions();

    public Type ModelType => typeof(TModel);

    public virtual Type? ParametersType => null;

    public virtual bool IsParametric => false;
}

public abstract class View<TModel> : ViewBase<TModel>
{
    public IQuerySource<TModel, TProperty> Include<TProperty>(Expression<Func<TModel, TProperty>> select)
    {
        var fieldKey = MemberOwnerUtilities.GetKey(select);

        return IncludeWithoutParameters<TProperty>(fieldKey);
    }

    public IQuerySource<TModel, TProperty> Include<TProperty>(Expression<Func<TModel, List<TProperty>>> select)
    {
        var fieldKey = MemberOwnerUtilities.GetKey(select);

        return IncludeWithoutParameters<TProperty>(fieldKey);
    }
}

public abstract class View<TParametersModel, TModel> : ViewBase<TModel>
{
    public override Type? ParametersType => typeof(TParametersModel);

    public override bool IsParametric => true;

    public IQuerySource<TParametersModel, TModel, TProperty> Include<TProperty>(
        Expression<Func<TModel, TProperty>> select)
    {
        var fieldKey = MemberOwnerUtilities.GetKey(select);

        return IncludeWithParameters<TParametersModel, TProperty>(fieldKey);
    }

    public IQuerySource<TParametersModel, TModel, TProperty> Include<TProperty>(
        Expression<Func<TModel, List<TProperty>>> select)
    {
        var fieldKey = MemberOwnerUtilities.GetKey(select);

        return IncludeWithParameters<TParametersModel, TProperty>(fieldKey);
    }
}

public abstract class FullTreeView<TModel> : View<TModel>
{
    private List<AccessNode> GetDescendants(AccessNode node)
    {
        var nodes = new List<AccessNode>();

        GetDescendants(node, nodes);

        return nodes;
    }

    private void GetDescendants(AccessNode node, List<AccessNode> nodes)
    {
        var children = node.GetChildren();

        foreach (var child in children)
        {
            nodes.Add(child);

            GetDescendants(child, nodes);
        }
    }

    protected override void MarkInclusions()
    {
        var ev = new ObjectEvaluator(typeof(TModel));

        var joinNodes = GetDescendants(ev.RootNode)
            .Where(n => !n.IsLeaf && !n.IsCollection && n != ev.RootNode)
            .ToList();

        foreach (var joinNode in joinNodes)
        {
            var inclusion = new InclusionRecord
            {
                Conditions = new List<InclusionCondition>(),
                Type = joinNode.Type,
                IncludedField = ev.Map.FieldKeyByNode(joinNode)
            };

            AddInclusion(inclusion);
        }
    }
}