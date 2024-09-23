using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Contracts;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.Snippets.Builtin;

public class SelectAllSnippet:ISnippet
{

    private readonly FilterQuery _filterQuery;
    private readonly OrderTerm[] _orders;
    private readonly bool _usePagination;
    private readonly bool _fullTree;
    private readonly Parameter _offsetParameter = Parameter.Null;
    private readonly Parameter _sizeParameter = Parameter.Null;
    
    public SelectAllSnippet(FilterQuery filterQuery, OrderTerm[] orders, Parameter offset,Parameter size, bool fullTree)
    {
        _filterQuery = filterQuery;
        _orders = orders;
        _usePagination = true;
        _offsetParameter = offset;
        _sizeParameter = size;
        _fullTree = fullTree;
    }
    
    public SelectAllSnippet(FilterQuery filterQuery, OrderTerm[] orders, bool fullTree)
    {
        _filterQuery = filterQuery;
        _orders = orders;
        _fullTree = fullTree;
        _usePagination = false;
        
    }

    public ISnippetToolbox Toolbox { get; set; } = ISnippetToolbox.Null;


    public string Pagination => _usePagination
        ? Toolbox.SqlTranslator.TranslatePagination(_offsetParameter, _sizeParameter)
        : string.Empty;

    public string Order => Toolbox.SqlTranslator.TranslateOrders(Toolbox.EffectiveType, _orders, _fullTree);

    public string Where =>
        Toolbox.SqlTranslator.TranslateFilterQueryToDbExpression(_filterQuery,
            ColumnNameTranslation.DataOwnerDotColumnName);

    public string Source => Toolbox.SourceName();

    public string Semicolon => Toolbox.Semicolon();
    
    public string Template => "Select * FROM {Source} {Where} {Order} {Pagination}{Semicolon}";
}