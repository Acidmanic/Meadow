using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Scaffolding.Snippets.Builtin.Models;

namespace Meadow.Scaffolding.Snippets.Builtin;

public class SelectSnippetParametersBuilder<TEntity>
{
    private readonly ISnippetToolbox _snippetToolbox;

    private Parameter _offset = Parameter.Null;
    private Parameter _size = Parameter.Null;
    private bool _usePagination = false;
    private bool _fullTree = false;
    private Type _entityType = typeof(TEntity);
    private ISnippet? _source;
    private bool _closeLine = true;

    private readonly List<Parameter> _inputParameters;
    private readonly List<Parameter> _byParameters;

    private Action<SnippetConfigurationBuilder> _manipulateConfigurations = scb => { };

    private FilterQueryBuilder<TEntity> _filterQueryBuilder = new FilterQueryBuilder<TEntity>();
    private OrderSetBuilder<TEntity> _orderSetBuilder = new OrderSetBuilder<TEntity>();

    public SelectSnippetParametersBuilder(ISnippetToolbox snippetToolbox)
    {
        _snippetToolbox = snippetToolbox;

        _byParameters = new List<Parameter>();
        _inputParameters = new List<Parameter>();
    }


    public SelectSnippetParametersBuilder<TEntity> Paginate(Parameter offset, Parameter size)
    {
        _offset = offset;
        _size = size;
        _inputParameters.Add(_offset);
        _inputParameters.Add(_size);
        _usePagination = true;
        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> Filter(Action<FilterQueryBuilder<TEntity>> filter)
    {
        filter(_filterQueryBuilder);

        return this;
    }


    public SelectSnippetParametersBuilder<TEntity> Order(Action<OrderSetBuilder<TEntity>> order)
    {
        order(_orderSetBuilder);

        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> FullTree()
    {
        _fullTree = true;

        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> EntityType(Type type)
    {
        _entityType = type;

        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> ManipulateConfigurations(
        Action<SnippetConfigurationBuilder> manipulateConfigurations)
    {
        _manipulateConfigurations = manipulateConfigurations;

        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> By(Action<IParameterSelector<TEntity>> select)
    {
        var parameterSelector = new ParameterSelector<TEntity>
        (_snippetToolbox.Construction.MeadowConfiguration,
            _snippetToolbox.TypeNameMapper, _entityType);

        select(parameterSelector);

        var parameters = parameterSelector.Build();

        _inputParameters.AddRange(parameters);

        _byParameters.AddRange(parameters);

        return this;
    }


    public SelectSnippetParametersBuilder<TEntity> Source(ISnippet source)
    {
        _source = source;

        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> CloseLine()
    {
        _closeLine = true;

        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> CloseLine(bool close)
    {
        _closeLine = close;

        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> Inline()
    {
        _closeLine = false;

        return this;
    }

    public SelectSnippetParameters Build()
    {
        var filterQuery = _filterQueryBuilder.Build();

        var orders = _orderSetBuilder.Build();


        Action<SnippetConfigurationBuilder> manipulate = _manipulateConfigurations;

        if (_entityType != typeof(TEntity))
        {
            manipulate = cb =>
            {
                cb.OverrideEntityType(_entityType);

                _manipulateConfigurations(cb);
            };
        }

        return new SelectSnippetParameters(filterQuery, orders, _usePagination,
            _fullTree, _entityType, manipulate, 
            _inputParameters, 
            _byParameters,_source, _closeLine, _offset, _size);
    }
}