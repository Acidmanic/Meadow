using System;
using System.Collections.Generic;
using System.Drawing;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Models;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.Snippets.Builtin.Models;

public class SelectSnippetParameters
{
    public FilterQuery FilterQuery { get; }
    public OrderTerm[] Orders { get; }
    public bool FullTree { get; }
    public Parameter? OffsetParameter { get; }
    public Parameter? SizeParameter { get; } 
    public Type EntityType { get; }
    public Action<SnippetConfigurationBuilder> ManipulateToolbox { get; }
    public List<Parameter> InputParameters { get; }
    public List<Parameter> ByParameters { get; }
    public ISnippet? OverrideSource { get; }
    
    public string? SourceAlias { get; }

    public List<SelectField> SelectFields { get; } = new();

    public bool CloseLine { get; }

    public SelectSnippetParameters(FilterQuery filterQuery, 
        OrderTerm[] orders, bool fullTree, Type entityType, 
        Action<SnippetConfigurationBuilder> manipulateToolbox, List<Parameter> inputParameters, 
        List<Parameter> byParameters, ISnippet? overrideSource, bool closeLine,
        Parameter? offsetParameter,Parameter? sizeParameter, string? sourceAlias, List<SelectField>? selectFields = null)
    {
        FilterQuery = filterQuery;
        Orders = orders;
        FullTree = fullTree;
        EntityType = entityType;
        ManipulateToolbox = manipulateToolbox;
        InputParameters = inputParameters;
        ByParameters = byParameters;
        OverrideSource = overrideSource;
        CloseLine = closeLine;
        OffsetParameter = offsetParameter;
        SizeParameter = sizeParameter;
        SourceAlias = sourceAlias;
        
        if(selectFields is {} sf ) SelectFields.AddRange(sf);
    }

    public bool IsSourceOverride => OverrideSource != null;

    public bool HasWhereClause => FilterQuery.NormalizedKeys().Count + ByParameters.Count > 0;

    public bool LimitsSize => SizeParameter != null;
    
    public bool SkipsRecords => OffsetParameter != null;

    public bool UsePagination => LimitsSize | SkipsRecords;
}