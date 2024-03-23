using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Inclusion.Enums;

namespace Meadow.Inclusion.Fluent.Markers;

internal class QueryTarget<TParametersModel,TModel, TProperty> : IToQueryTarget<TParametersModel,TModel, TProperty>, IThanQueryTarget<TParametersModel,TModel, TProperty>
{
    
    private readonly Action<TargetValueMark> _onSelect;

    private readonly IQuerySource<TParametersModel,TModel, TProperty> _source;

    public QueryTarget(Action<TargetValueMark> onSelect, IQuerySource<TParametersModel, TModel, TProperty> source)
    {
        _onSelect = onSelect;
        _source = source;
    }

    private IChainSelector<TParametersModel,TModel, TProperty> Select(TargetTypes targetType, Type? modelType, FieldKey? fieldKey, Type? valueType, string? value)
    {
        _onSelect(new TargetValueMark(targetType, modelType,fieldKey, valueType, value));

        return new ChainSelector<TParametersModel,TModel, TProperty>(_source);
    }
    
    private IChainSelector<TParametersModel, TModel, TProperty> Target<TValue>(TValue value)
    {
        return Select(TargetTypes.Constant,null,null, typeof(TValue), value?.ToString());
    }

    private IChainSelector<TParametersModel, TModel, TProperty> Target<TField>(Expression<Func<TModel, TField>> select)
    {
        var key = MemberOwnerUtilities.GetKey(select);
        
        return Select( TargetTypes.Field,typeof(TModel),key, null,null);
    }

    private IChainSelector<TParametersModel, TModel, TProperty> Target<TField>(Expression<Func<TProperty, TField>> select)
    {
        var key = MemberOwnerUtilities.GetKey(select);
        
        return Select(TargetTypes.Field, typeof(TProperty), key, null,null);
    }
    
    private IChainSelector<TParametersModel, TModel, TProperty> Target<TField>(Expression<Func<TParametersModel, TField>> select)
    {
        var key = MemberOwnerUtilities.GetKey(select);
        
        return Select(TargetTypes.Parameter, typeof(TProperty), key, null,null);
    }

    public IChainSelector<TParametersModel, TModel, TProperty> To<TValue>(TValue value) => Target(value);

    public IChainSelector<TParametersModel, TModel, TProperty> To<TField>(Expression<Func<TModel, TField>> select) => Target(select);

    public IChainSelector<TParametersModel, TModel, TProperty> To<TField>(Expression<Func<TProperty, TField>> select) => Target(select);

    public IChainSelector<TParametersModel, TModel, TProperty> To<TParameter>(Expression<Func<TParametersModel, TParameter>> select) => Target(select);
    
    public IChainSelector<TParametersModel, TModel, TProperty> Than<TValue>(TValue value)=> Target(value);

    public IChainSelector<TParametersModel, TModel, TProperty> Than<TField>(Expression<Func<TModel, TField>> select)=> Target(select);

    public IChainSelector<TParametersModel, TModel, TProperty> Than<TField>(Expression<Func<TProperty, TField>> select)=> Target(select);

    public IChainSelector<TParametersModel, TModel, TProperty> Than<TParameter>(Expression<Func<TParametersModel, TParameter>> select)=> Target(select);
}


internal class QueryTarget<TModel, TProperty> : IToQueryTarget<TModel, TProperty>, IThanQueryTarget<TModel, TProperty>
{

    private readonly Action<TargetValueMark> _onSelect;

    private readonly IQuerySource<TModel, TProperty> _source;
    public QueryTarget(Action<TargetValueMark> onSelect, IQuerySource<TModel, TProperty> source)
    {
        _onSelect = onSelect;
        _source = source;
    }

    private IChainSelector<TModel, TProperty> Select(TargetTypes targetType, Type? modelType, FieldKey? fieldKey, Type? valueType, string? value)
    {
        _onSelect(new TargetValueMark( targetType,modelType,fieldKey, valueType, value));

        return new ChainSelector<TModel, TProperty>(_source);
    }

    private IChainSelector<TModel, TProperty> Target<TValue>(TValue value)
    {
        return Select(TargetTypes.Constant,null,null, typeof(TValue), value?.ToString());
    }

    private IChainSelector<TModel, TProperty> Target<TField>(Expression<Func<TModel, TField>> select)
    {
        var key = MemberOwnerUtilities.GetKey(select);
        
        return Select( TargetTypes.Field,typeof(TModel),key, null,null);
    }

    private IChainSelector<TModel, TProperty> Target<TField>(Expression<Func<TProperty, TField>> select)
    {
        var key = MemberOwnerUtilities.GetKey(select);
        
        return Select(TargetTypes.Field, typeof(TProperty), key, null,null);
    }

    public IChainSelector<TModel, TProperty> To<TValue>(TValue value) => Target(value);

    public IChainSelector<TModel, TProperty> To<TField>(Expression<Func<TModel, TField>> select) => Target(select);
    
    public IChainSelector<TModel, TProperty> To<TField>(Expression<Func<TProperty, TField>> select) => Target(select);

    public IChainSelector<TModel, TProperty> Than<TValue>(TValue value) => Target(value);

    public IChainSelector<TModel, TProperty> Than<TField>(Expression<Func<TModel, TField>> select) => Target(select);

    public IChainSelector<TModel, TProperty> Than<TField>(Expression<Func<TProperty, TField>> select) => Target(select);

}