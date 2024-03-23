using System;
using Meadow.Inclusion.Enums;

namespace Meadow.Inclusion.Fluent.Markers;


internal class ChainSelector<TParametersModel,TModel, TProperty> : IChainSelector<TParametersModel,TModel, TProperty>
{
    private readonly IQuerySource<TParametersModel,TModel, TProperty> _source;

    private readonly Action<BooleanRelation> _onRelateToNext;
    public ChainSelector(IQuerySource<TParametersModel, TModel, TProperty> source, Action<BooleanRelation> onRelateToNext)
    {
        _source = source;
        _onRelateToNext = onRelateToNext;
    }

    public IQuerySource<TParametersModel, TModel, TProperty> And()
    {

        _onRelateToNext(BooleanRelation.And);
        
        return _source;
    }

    public IQuerySource<TParametersModel, TModel, TProperty> Or()
    {
        _onRelateToNext(BooleanRelation.Or);
        
        return _source;
    }
}

internal class ChainSelector<TModel,TProperty>:IChainSelector<TModel,TProperty>
{

    private readonly IQuerySource<TModel, TProperty> _source;
    private readonly Action<BooleanRelation> _onRelateToNext;
    
    public ChainSelector(IQuerySource<TModel, TProperty> source, Action<BooleanRelation> onRelateToNext)
    {
        _source = source;
        _onRelateToNext = onRelateToNext;
    }

    public IQuerySource<TModel, TProperty> And()
    {
        _onRelateToNext(BooleanRelation.And);
        
        return _source;
    }

    public IQuerySource<TModel, TProperty> Or()
    {
        _onRelateToNext(BooleanRelation.Or);
        
        return _source;
    }
}