using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.Snippets.Builtin;

public class ReadAllProcedureSnippetBuilder<TEntity>
{
    private readonly string _procedureName;
    private readonly SnippetToolbox _snippetToolbox;

    private Parameter _offset = Parameter.Null;
    private Parameter _size = Parameter.Null;
    private bool _usePagination = false;
    private bool _fullTree = false;
    private Type _entityType = typeof(TEntity);

    private readonly List<Parameter> _inputParameters;
    private readonly List<Parameter> _byParameters;

    private Action<SnippetConfigurationBuilder> _manipulateConfigurations = scb => { };

    private FilterQueryBuilder<TEntity> _filterQueryBuilder = new FilterQueryBuilder<TEntity>();
    private OrderSetBuilder<TEntity> _orderSetBuilder = new OrderSetBuilder<TEntity>();

    public ReadAllProcedureSnippetBuilder(string procedureName, SnippetToolbox snippetToolbox)
    {
        _procedureName = procedureName;
        _snippetToolbox = snippetToolbox;
    }


    public ReadAllProcedureSnippetBuilder<TEntity> Paginate(Parameter offset, Parameter size)
    {
        _offset = offset;
        _size = size;
        _inputParameters.Add(_offset);
        _inputParameters.Add(_size);
        _usePagination = true;
        return this;
    }

    public ReadAllProcedureSnippetBuilder<TEntity> Filter(Action<FilterQueryBuilder<TEntity>> filter)
    {
        filter(_filterQueryBuilder);

        return this;
    }


    public ReadAllProcedureSnippetBuilder<TEntity> Order(Action<OrderSetBuilder<TEntity>> order)
    {
        order(_orderSetBuilder);

        return this;
    }

    public ReadAllProcedureSnippetBuilder<TEntity> FullTree()
    {
        _fullTree = true;

        return this;
    }

    public ReadAllProcedureSnippetBuilder<TEntity> EntityType(Type type)
    {
        _entityType = type;
        return this;
    }

    public ReadAllProcedureSnippetBuilder<TEntity> ManipulateConfigurations(Action<SnippetConfigurationBuilder> manipulateConfigurations)
    {
        _manipulateConfigurations = manipulateConfigurations;

        return this;
    }

    public ReadAllProcedureSnippetBuilder<TEntity> By(Action<IParameterSelector<TEntity>> select)
    {
        var parameterSelector = new ParameterSelector<TEntity>(_snippetToolbox.Construction.MeadowConfiguration, _snippetToolbox.TypeNameMapper);

        select(parameterSelector);

        var parameters = parameterSelector.Build();

        _inputParameters.AddRange(parameters);

        _byParameters.AddRange(parameters);

        return this;
    }


    public ReadAllProcedureSnippet Build()
    {
        var filterQuery = _filterQueryBuilder.Build();

        var orders = _orderSetBuilder.Build();

        if (_usePagination)
        {
            return new ReadAllProcedureSnippet(filterQuery, orders, _fullTree, _entityType,
                _manipulateConfigurations, _inputParameters, _byParameters,
                _procedureName, _offset, _size);
        }

        return new ReadAllProcedureSnippet(filterQuery, orders, _fullTree, _entityType, _manipulateConfigurations, _inputParameters, _byParameters, _procedureName);
    }
}