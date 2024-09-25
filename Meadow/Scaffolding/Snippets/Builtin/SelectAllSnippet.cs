using System;
using System.ComponentModel;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Contracts;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.Snippets.Builtin;

public class SelectAllSnippet : ISnippet
{
    private readonly FilterQuery _filterQuery;
    private readonly OrderTerm[] _orders;
    private readonly bool _usePagination;
    private readonly bool _fullTree;
    private readonly Parameter _offsetParameter = Parameter.Null;
    private readonly Parameter _sizeParameter = Parameter.Null;
    private readonly Type _entityType;
    private readonly Action<SnippetConfigurationBuilder> _manipulateToolbox;

    public SelectAllSnippet(Type entityType, FilterQuery filterQuery, OrderTerm[] orders, Parameter offset,
        Parameter size, bool fullTree, Action<SnippetConfigurationBuilder>? manipulateToolbox = null)
    {
        _filterQuery = filterQuery;
        _orders = orders;
        _usePagination = true;
        _offsetParameter = offset;
        _sizeParameter = size;
        _fullTree = fullTree;
        _entityType = entityType;
        _manipulateToolbox = manipulateToolbox ?? (t => { });
    }

    public SelectAllSnippet(Type entityType, FilterQuery filterQuery, OrderTerm[] orders, bool fullTree, Action<SnippetConfigurationBuilder>? manipulateToolbox = null)
    {
        _filterQuery = filterQuery;
        _orders = orders;
        _fullTree = fullTree;
        _entityType = entityType;
        _usePagination = false;
        _manipulateToolbox = manipulateToolbox ?? (t => { });
    }

    public ISnippetToolbox Toolbox { get; set; } = ISnippetToolbox.Null;


    private ISnippetToolbox T
    {
        get
        {
            var t = Toolbox.CloneFor(_entityType);

            var b = new SnippetConfigurationBuilder(t.Configurations);

            _manipulateToolbox(b);

            return new SnippetToolbox(t.Construction, b.Build());
        }
    }

    public string Pagination => _usePagination
        ? T.SqlTranslator.TranslatePagination(_offsetParameter, _sizeParameter)
        : string.Empty;

    public string Order => _orders.Any() ? " ORDER BY " + T.SqlTranslator.TranslateOrders(T.EffectiveType, _orders, _fullTree) : string.Empty;

    public string WhereFilter =>
        T.SqlTranslator.TranslateFilterQueryToDbExpression(_filterQuery,
            ColumnNameTranslation.DataOwnerDotColumnName, T.SourceName(_fullTree));

    public string WhereKeyword => _filterQuery.NormalizedKeys().Count > 0 ? " WHERE " : string.Empty;
    public string Source => T.SourceName();

    public string Semicolon => T.Semicolon();

    public string Template => "Select * FROM {Source}{WhereKeyword}{WhereFilter}{Order}{Pagination}{Semicolon}";


    public static SelectAllSnippet Create<TBuildersArgument>(
        Type actualEntityType,
        Parameter offset, Parameter size,
        Action<FilterQueryBuilder<TBuildersArgument>>? filter = null,
        Action<OrderSetBuilder<TBuildersArgument>>? order = null,
        bool fullTree = false)
    {
        Action<FilterQueryBuilder<TBuildersArgument>> fb = filter ?? (b => { });
        Action<OrderSetBuilder<TBuildersArgument>> ob = order ?? (o => { });


        var filterBuilder = new FilterQueryBuilder<TBuildersArgument>();
        var orderBuilder = new OrderSetBuilder<TBuildersArgument>();

        fb(filterBuilder);
        ob(orderBuilder);

        var filterQuery = filterBuilder.Build();
        var orders = orderBuilder.Build();

        return new SelectAllSnippet(actualEntityType, filterQuery, orders, offset, size, fullTree);
    }

    public static SelectAllSnippet Create<TEntity>(
        Parameter offset, Parameter size,
        Action<FilterQueryBuilder<TEntity>>? filter = null,
        Action<OrderSetBuilder<TEntity>>? order = null,
        bool fullTree = false) => Create<TEntity>(typeof(TEntity),
        offset, size, filter, order, fullTree);

    public static SelectAllSnippet Create<TBuildersArgument>(
        Type actualEntityType,
        Action<FilterQueryBuilder<TBuildersArgument>>? filter = null,
        Action<OrderSetBuilder<TBuildersArgument>>? order = null,
        bool fullTree = false, Action<SnippetConfigurationBuilder>? manipulateToolbox = null)
    {
        Action<FilterQueryBuilder<TBuildersArgument>> fb = filter ?? (b => { });
        Action<OrderSetBuilder<TBuildersArgument>> ob = order ?? (o => { });


        var filterBuilder = new FilterQueryBuilder<TBuildersArgument>();
        var orderBuilder = new OrderSetBuilder<TBuildersArgument>();

        fb(filterBuilder);
        ob(orderBuilder);

        var filterQuery = filterBuilder.Build();
        var orders = orderBuilder.Build();

        return new SelectAllSnippet(actualEntityType, filterQuery, orders, fullTree, manipulateToolbox);
    }

    public static SelectAllSnippet Create<TEntity>(
        Action<FilterQueryBuilder<TEntity>>? filter = null,
        Action<OrderSetBuilder<TEntity>>? order = null,
        bool fullTree = false, Action<SnippetConfigurationBuilder>? manipulateToolbox = null)
        => Create(typeof(TEntity), filter, order, fullTree, manipulateToolbox);
}