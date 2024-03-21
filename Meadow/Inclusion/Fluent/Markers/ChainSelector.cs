namespace Meadow.Inclusion.Fluent.Markers;

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