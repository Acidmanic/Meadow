using System.Linq;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets.Builtin.Models;

namespace Meadow.Scaffolding.Snippets.Builtin;

public class ReadAllSelectSnippet : ISnippet
{
    private readonly SelectSnippetParameters _parameters;

    public ReadAllSelectSnippet(SelectSnippetParameters parameters)
    {
        _parameters = parameters;
    }


    public ISnippetToolbox Toolbox { get; set; } = ISnippetToolbox.Null;


    private ISnippetToolbox T
    {
        get
        {
            var t = Toolbox.CloneFor(_parameters.EntityType);

            var b = new SnippetConfigurationBuilder(t.Configurations);

            _parameters.ManipulateToolbox(b);

            return new SnippetToolbox(t.Construction, b.Build());
        }
    }

    public ISnippet Source => _parameters.OverrideSource ?? new StringSnippet(T.SourceName());

    public string Sop => _parameters.SourceInParentheses ? "(" : string.Empty;
    public string Scp => _parameters.SourceInParentheses ? ")" : string.Empty;

    public string SourceAlias => _parameters.SourceInParentheses ? 
        " " + T.SqlTranslator
        .AliasTableName(T.SqlTranslator.QuoteTable(_parameters.SourceAlias ?? string.Empty)) : string.Empty;
    
    public string Pagination => _parameters.UsePagination
        ? T.SqlTranslator.TranslatePagination(_parameters.OffsetParameter, _parameters.SizeParameter)
        : string.Empty;

    public string Order => _parameters.Orders.Any() ? " ORDER BY " + T.SqlTranslator.TranslateOrders(T.EffectiveType, _parameters.Orders, _parameters.FullTree) : string.Empty;

    public string WhereFilter =>
        T.SqlTranslator.TranslateFilterQueryToDbExpression(_parameters.FilterQuery,
            ColumnNameTranslation.DataOwnerDotColumnName, T.SourceName(_parameters.FullTree));

    public string WhereBy => T.EqualityClause(fullTree: _parameters.FullTree, parameters: _parameters.ByParameters.ToArray());

    public string ByToFilter => _parameters.ByParameters.Count > 0 && _parameters.FilterQuery.NormalizedKeys().Count > 0 ? " AND " : string.Empty;

    public string WhereKeyword => _parameters.HasWhereClause? " WHERE " : string.Empty;

    public string Semicolon => _parameters.CloseLine ? T.Semicolon() : string.Empty;

    public string Template => "Select * FROM {Sop}{Source}{Scp}{SourceAlias}{WhereKeyword}{WhereBy}{ByToFilter}{WhereFilter}{Order}{Pagination}{Semicolon}";
}