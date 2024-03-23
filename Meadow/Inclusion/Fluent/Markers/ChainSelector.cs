namespace Meadow.Inclusion.Fluent.Markers;


internal class ChainSelector<TParametersModel,TModel, TProperty> : IChainSelector<TParametersModel,TModel, TProperty>
{
    private readonly IQuerySource<TParametersModel,TModel, TProperty> _source;

    public ChainSelector(IQuerySource<TParametersModel, TModel, TProperty> source)
    {
        _source = source;
    }

    public IQuerySource<TParametersModel, TModel, TProperty> And()
    {
        return _source;
    }

    public IQuerySource<TParametersModel, TModel, TProperty> Or()
    {
        return _source;
    }
}

internal class ChainSelector<TModel,TProperty>:IChainSelector<TModel,TProperty>
{

    private readonly IQuerySource<TModel, TProperty> _source;

    public ChainSelector(IQuerySource<TModel, TProperty> source)
    {
        _source = source;
    }

    public IQuerySource<TModel, TProperty> And()
    {
        return _source;
    }

    public IQuerySource<TModel, TProperty> Or()
    {
        return _source;
    }
}