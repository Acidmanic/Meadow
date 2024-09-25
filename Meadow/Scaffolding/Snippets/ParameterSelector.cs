using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Models;
using Meadow.Utility;

namespace Meadow.Scaffolding.Snippets;


public interface ISelectedParametersBuilder
{
    Parameter[] Build();
}

public interface IParameterSelector<TEntity>
{
    IParameterSelector<TEntity> Add(Expression<Func<TEntity, object>> select,Type? overrideType = null,bool fullTree = false);
}

public interface IParameterSelector
{
    IParameterSelector Add<TEntity>(Expression<Func<TEntity, object>> select,Type? overrideType = null,bool fullTree = false);
    
}

public class ParameterSelector:IParameterSelector,ISelectedParametersBuilder
{

    private readonly IDbTypeNameMapper _dbTypeNameMapper;
    private readonly MeadowConfiguration _meadowConfiguration;
    private readonly List<Parameter> _parameters;
    
    public ParameterSelector(MeadowConfiguration meadowConfiguration,IDbTypeNameMapper dbTypeNameMapper)
    {
        _dbTypeNameMapper = dbTypeNameMapper;
        _meadowConfiguration = meadowConfiguration;
        _parameters = new List<Parameter>();
    }

    public IParameterSelector Add<TEntity>(Expression<Func<TEntity, object>> select, Type? overrideType = null,bool fullTree = false)
    {
        var effectiveType = overrideType ?? typeof(TEntity);

        var processedType = EntityTypeUtilities.Process(effectiveType,_meadowConfiguration,_dbTypeNameMapper);

        var allParameters = fullTree ? processedType.ParametersFullTree: processedType.Parameters;
        
        var address = MemberOwnerUtilities.GetAddress(select);

        var parameter = allParameters.FirstOrDefault(p => p.StandardAddress == address);

        if (parameter is { } par)
        {
            _parameters.Add(par);
        }

        return this;
    }

    public void Clear()
    {
        _parameters.Clear();
    }

    public Parameter[] Build()
    {
        return _parameters.ToArray();
    }
}

public class ParameterSelector<TEntity>:IParameterSelector<TEntity>,ISelectedParametersBuilder
{

    private readonly IDbTypeNameMapper _dbTypeNameMapper;
    private readonly MeadowConfiguration _meadowConfiguration;
    private readonly List<Parameter> _parameters;
    
    public ParameterSelector(MeadowConfiguration meadowConfiguration,IDbTypeNameMapper dbTypeNameMapper)
    {
        _dbTypeNameMapper = dbTypeNameMapper;
        _meadowConfiguration = meadowConfiguration;
        _parameters = new List<Parameter>();
    }

    public IParameterSelector<TEntity> Add(Expression<Func<TEntity, object>> select, Type? overrideType = null,bool fullTree = false)
    {
        var effectiveType = overrideType ?? typeof(TEntity);

        var processedType = EntityTypeUtilities.Process(effectiveType,_meadowConfiguration,_dbTypeNameMapper);

        var allParameters = fullTree ? processedType.ParametersFullTree: processedType.Parameters;
        
        var address = MemberOwnerUtilities.GetAddress(select);

        var parameter = allParameters.FirstOrDefault(p => p.StandardAddress == address);

        if (parameter is { } par)
        {
            _parameters.Add(par);
        }

        return this;
    }

    public void Clear()
    {
        _parameters.Clear();
    }

    public Parameter[] Build()
    {
        return _parameters.ToArray();
    }
}
