using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Enums;
using Meadow.Models;
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
    private string _sourceAlias = string.Empty;
    private bool _closeLine = true;

    private readonly List<Parameter> _inputParameters;
    private readonly List<Parameter> _byParameters;

    private Action<SnippetConfigurationBuilder> _manipulateConfigurations = scb => { };

    private FilterQueryBuilder<TEntity> _filterQueryBuilder = new FilterQueryBuilder<TEntity>();
    private OrderSetBuilder<TEntity> _orderSetBuilder = new OrderSetBuilder<TEntity>();

    private readonly List<SelectField> _selectFields = new();

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
        _usePagination = true;
        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> Offset(Parameter offset)
    {
        _offset = offset;
        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> Size(Parameter size)
    {
        _size = size;
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

    public SelectSnippetParametersBuilder<TEntity> InputParameters(params Parameter[] inputParameters)
    {
        _inputParameters.AddRange(inputParameters);

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
        var parameters = SelectParameters(select);

        _byParameters.AddRange(parameters);

        return this;
    }

    private Parameter[] SelectParameters(Action<IParameterSelector<TEntity>> select)
    {
        var parameterSelector = new ParameterSelector<TEntity>
        (_snippetToolbox.Construction.MeadowConfiguration,
            _snippetToolbox.TypeNameMapper, _entityType);

        select(parameterSelector);

        var parameters = parameterSelector.Build();

        return parameters;
    }

    public SelectSnippetParametersBuilder<TEntity> Source(ISnippet source, string alias)
    {
        _source = source;

        _sourceAlias = alias;

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

    public SelectSnippetParametersBuilder<TEntity> SelectAll()
    {
        _selectFields.Clear();

        return this;
    }


    public SelectSnippetParametersBuilder<TEntity> Select(string code, SelectFieldType type, string? alias = null)
    {
        _selectFields.Add(new SelectField
        {
            Alias = alias,
            Code = code,
            Type = type
        });

        return this;
    }

    public SelectSnippetParametersBuilder<TEntity> SelectColumns(Action<IParameterSelector<TEntity>> select)
    {
        var parameters = SelectParameters(select);

        foreach (var parameter in parameters)
        {
            _selectFields.Add(new SelectField
            {
                Alias = null,
                Code = parameter.Name,
                Type = SelectFieldType.ColumnName
            });
        }

        return this;
    }


    private List<Parameter> ExtractParameters(FilterQuery query)
    {
        var items = query.Items();

        var parameters = new Dictionary<string, Parameter>();

        void ExtractParameter(object o)
        {
            if (o is Parameter p && !parameters.ContainsKey(p.StandardAddress))
            {
                parameters.Add(p.StandardAddress, p);
            }
        }

        foreach (var item in items)
        {
            foreach (var ev in item.EqualityValues)
            {
                ExtractParameter(ev);
            }

            ExtractParameter(item.Minimum);
            ExtractParameter(item.Maximum);
        }

        return parameters.Values.ToList();
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

            filterQuery.EntityType = _entityType;
        }

        var inputs = new List<Parameter>(_inputParameters);

        if (_usePagination)
        {
            inputs.Add(_offset);
            inputs.Add(_size);
        }

        inputs.AddRange(_byParameters);

        inputs.AddRange(ExtractParameters(filterQuery));

        return new SelectSnippetParameters(filterQuery, orders, _fullTree, _entityType, manipulate,
            inputs, _byParameters, _source,
            _closeLine, _offset, _size, _sourceAlias, _selectFields);
    }
}