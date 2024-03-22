using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Inclusion.Fluent.Markers;

internal class QueryTarget<TModel, TProperty> : IToQueryTarget<TModel, TProperty>, IThanQueryTarget<TModel, TProperty>
{

    private readonly Action<TargetValueMark> _onSelect;

    private readonly IQuerySource<TModel, TProperty> _source;
    public QueryTarget(Action<TargetValueMark> onSelect, IQuerySource<TModel, TProperty> source)
    {
        _onSelect = onSelect;
        _source = source;
    }

    private IChainSelector<TModel, TProperty> Select(Type modelType, FieldKey? fieldKey, bool isConstant,Type? valueType, string? value)
    {
        _onSelect(new TargetValueMark(modelType,fieldKey, isConstant, valueType, value));

        return new ChainSelector<TModel, TProperty>(_source);
    }

    private IChainSelector<TModel, TProperty> Target<TValue>(TValue value)
    {
        return Select(null,null, true,typeof(TValue), value?.ToString());
    }

    private IChainSelector<TModel, TProperty> Target<TField>(Expression<Func<TModel, TField>> select)
    {
        var key = MemberOwnerUtilities.GetKey(select);
        
        return Select(typeof(TModel),key, false, null,null);
    }

    private IChainSelector<TModel, TProperty> Target<TField>(Expression<Func<TProperty, TField>> select)
    {
        var key = MemberOwnerUtilities.GetKey(select);
        
        return Select(typeof(TProperty), key, false, null,null);
    }

    public IChainSelector<TModel, TProperty> To<TValue>(TValue value) => Target(value);

    public IChainSelector<TModel, TProperty> To<TField>(Expression<Func<TModel, TField>> select) => Target(select);
    
    public IChainSelector<TModel, TProperty> To<TField>(Expression<Func<TProperty, TField>> select) => Target(select);

    public IChainSelector<TModel, TProperty> Than(string value) => Target(value);

    public IChainSelector<TModel, TProperty> Than<TField>(Expression<Func<TModel, TField>> select) => Target(select);

    public IChainSelector<TModel, TProperty> Than<TField>(Expression<Func<TProperty, TField>> select) => Target(select);

}