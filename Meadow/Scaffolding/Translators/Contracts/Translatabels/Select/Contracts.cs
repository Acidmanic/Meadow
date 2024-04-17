using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.Translators.Contracts.Translatabels.Select;


public interface ISelectBuilder
{
    
    IAlsoSelectBuilder Select(Parameter parameter);
    
    IAlsoSelectBuilder Select<TModel,TProperty>(Expression<Func<TModel,TProperty>> parameter);
    
    IAlsoSelectBuilder Select(FieldKey key);
    
    IAlsoSelectBuilder Select(string value,string? alias = null);
    
}

public interface IAlsoSelectBuilder
{
    
    IAlsoSelectBuilder AlsoSelect(Parameter parameter);
    
    IAlsoSelectBuilder AlsoSelect<TModel,TProperty>(Expression<Func<TModel,TProperty>> parameter);
    
    IAlsoSelectBuilder AlsoSelect(FieldKey key);
    
    IAlsoSelectBuilder AlsoSelect(string value,string? alias = null);

    IWhereBuilder From(Type type);
}


public interface IWhereBuilder
{
    IOperatorSelector Where(Parameter parameter);
    
    IOperatorSelector Where<TModel,TProperty>(Expression<Func<TModel,TProperty>> parameter);
    
    IOperatorSelector Where(FieldKey key);
    
    IOperatorSelector Where(string value);
    
}


public interface IOperatorSelector
{
    IToQueryTarget IsEqual();

    IToQueryTarget IsNotEqual();

    IThanQueryTarget IsGreater();

    IThanQueryTarget IsSmaller();
    
    IToQueryTarget IsGreaterOrEqual();

    IToQueryTarget IsSmallerOrEqual();
}

public interface IToQueryTarget
{
    IConditionChainBuilder To(Parameter parameter);
    
    IConditionChainBuilder To<TModel,TProperty>(Expression<Func<TModel,TProperty>> parameter);
    
    IConditionChainBuilder To(FieldKey key);
    
    IConditionChainBuilder To(string value);
   
}


public interface IThanQueryTarget
{
    IConditionChainBuilder Than(Parameter parameter);
    
    IConditionChainBuilder Than<TModel,TProperty>(Expression<Func<TModel,TProperty>> parameter);
    
    IConditionChainBuilder Than(FieldKey key);
    
    IConditionChainBuilder Than(string value);
}

public interface IConditionChainBuilder
{
    IWhereBuilder And();
    
    IWhereBuilder Or();
}