using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Contracts;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.Snippets.Builtin;

public class ReadAllProcedureSnippet : ISnippet
{
    private readonly FilterQuery _filterQuery;
    private readonly OrderTerm[] _orders;
    private readonly bool _usePagination;
    private readonly bool _fullTree;
    private readonly Parameter _offsetParameter = Parameter.Null;
    private readonly Parameter _sizeParameter = Parameter.Null;
    private readonly Type _entityType;
    private readonly Action<SnippetConfigurationBuilder> _manipulateToolbox;
    private readonly string _procedureName; 
    private readonly List<Parameter> _inputParameters;
    private readonly List<Parameter> _byParameters;


    public ReadAllProcedureSnippet(FilterQuery filterQuery, 
        OrderTerm[] orders, 
        bool fullTree, 
        Type entityType, 
        Action<SnippetConfigurationBuilder> manipulateToolbox, 
        List<Parameter> inputParameters, 
        List<Parameter> byParameters,
        string procedureName,
        Parameter offsetParameter,
        Parameter sizeParameter)
    {
        _filterQuery = filterQuery;
        _orders = orders;
        _usePagination = true;
        _fullTree = fullTree;
        _entityType = entityType;
        _manipulateToolbox = manipulateToolbox;
        _inputParameters = inputParameters;
        _byParameters = byParameters;
        _offsetParameter = offsetParameter;
        _sizeParameter = sizeParameter;
        _procedureName = procedureName;
        inputParameters.AddRange(new Parameter[]{_offsetParameter,_sizeParameter});
    }
    public ReadAllProcedureSnippet(FilterQuery filterQuery, 
        OrderTerm[] orders, 
        bool fullTree, 
        Type entityType, 
        Action<SnippetConfigurationBuilder> manipulateToolbox, 
        List<Parameter> inputParameters, 
        List<Parameter> byParameters,
        string procedureName)
    {
        _filterQuery = filterQuery;
        _orders = orders;
        _usePagination = false;
        _fullTree = fullTree;
        _entityType = entityType;
        _manipulateToolbox = manipulateToolbox;
        _inputParameters = inputParameters;
        _byParameters = byParameters;
        _procedureName = procedureName;
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

    

    public string WhereBy => T.EqualityClause(fullTree: _fullTree, parameters: _byParameters.ToArray());

    public string ByToFilter => _byParameters.Count > 0 && _filterQuery.NormalizedKeys().Count > 0 ? " AND " : string.Empty;
    
    public string WhereKeyword => 
        _filterQuery.NormalizedKeys().Count + _byParameters.Count
                                  > 0 ? " WHERE " : string.Empty;
    
    public string Source => T.SourceName();

    public string Semicolon => T.Semicolon();

    public string Procedure(string body) => Toolbox.Procedure(T.Configurations.RepetitionHandling,_procedureName,
        body,string.Empty,T.SourceName(),_inputParameters.ToArray());

    public ISnippet Line => new CommentLineSnippet();
    
    public string Template => @"
{Procedure}
    Select * FROM {Source}{WhereKeyword}{WhereBy}{ByToFilter}{WhereFilter}{Order}{Pagination}{Semicolon}
{/Procedure}
{Line}
";


}