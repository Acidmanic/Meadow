using System;
using System.Linq.Expressions;

namespace Meadow.Inclusion.Fluent;

public interface IQuerySource<TModel,TProperty>
{

    IOperatorSelector<TModel,TProperty> Where<TField>(Expression<Func<TProperty, TField>> select);
}

public interface IOperatorSelector<TModel,TProperty>
{
    IToQueryTarget<TModel,TProperty> IsEqual();

    IToQueryTarget<TModel,TProperty> IsNotEqual();

    
    IThanQueryTarget<TModel,TProperty> IsGreater();
    
    
    IThanQueryTarget<TModel,TProperty> IsSmaller();
    
    IToQueryTarget<TModel,TProperty> IsGreaterOrEqual();

    IToQueryTarget<TModel,TProperty> IsSmallerOrEqual();
}


public interface IToQueryTarget<TModel,TProperty>
{
    
    IChainSelector<TModel,TProperty> To<TValue>(TValue value);

    IChainSelector<TModel,TProperty> To<TField>(Expression<Func<TModel, TField>> select);
    
    IChainSelector<TModel,TProperty> To<TField>(Expression<Func<TProperty, TField>> select);
}

public interface IThanQueryTarget<TModel,TProperty>
{
    IChainSelector<TModel,TProperty> Than(string value);
    
    IChainSelector<TModel,TProperty> Than<TField>(Expression<Func<TModel, TField>> select);
    
    IChainSelector<TModel,TProperty> Than<TField>(Expression<Func<TProperty, TField>> select);
}

public interface IChainSelector<TModel,TProperty>
{
    IQuerySource<TModel,TProperty> And();

    IQuerySource<TModel,TProperty> Or();
    
}
