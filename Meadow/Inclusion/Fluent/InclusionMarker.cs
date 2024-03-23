using System;
using System.Linq.Expressions;

namespace Meadow.Inclusion.Fluent;

public interface IQuerySource<TModel,TProperty>
{

    IOperatorSelector<TModel,TProperty> Where<TField>(Expression<Func<TProperty, TField>> select);
}

public interface IQuerySource<TParametersModel,TModel,TProperty>
{

    IOperatorSelector<TParametersModel,TModel,TProperty> Where<TField>(Expression<Func<TProperty, TField>> select);
}

public interface IOperatorSelector<TParametersModel,TModel,TProperty>
{
    IToQueryTarget<TParametersModel,TModel,TProperty> IsEqual();

    IToQueryTarget<TParametersModel,TModel,TProperty> IsNotEqual();

    
    IThanQueryTarget<TParametersModel,TModel,TProperty> IsGreater();
    
    
    IThanQueryTarget<TParametersModel,TModel,TProperty> IsSmaller();
    
    IToQueryTarget<TParametersModel,TModel,TProperty> IsGreaterOrEqual();

    IToQueryTarget<TParametersModel,TModel,TProperty> IsSmallerOrEqual();
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


public interface IToQueryTarget<TParametersModel, TModel,TProperty>
{
    
    IChainSelector<TParametersModel,TModel,TProperty> To<TValue>(TValue value);
    
    IChainSelector<TParametersModel,TModel,TProperty> To<TParameter>(Expression<Func<TParametersModel,TParameter>> select);

    IChainSelector<TParametersModel,TModel,TProperty> To<TField>(Expression<Func<TModel, TField>> select);
    
    IChainSelector<TParametersModel,TModel,TProperty> To<TField>(Expression<Func<TProperty, TField>> select);
}

public interface IToQueryTarget< TModel,TProperty>
{
    
    IChainSelector<TModel,TProperty> To<TValue>(TValue value);
    
    IChainSelector<TModel,TProperty> To<TField>(Expression<Func<TModel, TField>> select);
    
    IChainSelector<TModel,TProperty> To<TField>(Expression<Func<TProperty, TField>> select);
}

public interface IThanQueryTarget<TParametersModel,TModel,TProperty>
{
    IChainSelector<TParametersModel,TModel,TProperty> Than<TValue>(TValue value);
    
    IChainSelector<TParametersModel,TModel,TProperty> Than<TField>(Expression<Func<TModel, TField>> select);
    
    IChainSelector<TParametersModel,TModel,TProperty> Than<TField>(Expression<Func<TProperty, TField>> select);
    
    IChainSelector<TParametersModel,TModel,TProperty> Than<TParameter>(Expression<Func<TParametersModel,TParameter>> select);
}

public interface IThanQueryTarget<TModel,TProperty>
{
    IChainSelector<TModel,TProperty> Than<TValue>(TValue value);
    
    IChainSelector<TModel,TProperty> Than<TField>(Expression<Func<TModel, TField>> select);
    
    IChainSelector<TModel,TProperty> Than<TField>(Expression<Func<TProperty, TField>> select);
    
}

public interface IChainSelector<TParametersModel,TModel,TProperty>
{
    IQuerySource<TParametersModel,TModel,TProperty> And();

    IQuerySource<TParametersModel,TModel,TProperty> Or();
    
}

public interface IChainSelector<TModel,TProperty>
{
    IQuerySource<TModel,TProperty> And();

    IQuerySource<TModel,TProperty> Or();
    
}