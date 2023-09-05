using System;
using Acidmanic.Utilities.Results;
using Meadow.Configuration;

namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets;

public class SnippetConfigurations
{
    
    /// <summary>
    /// This property is not being used by the Snippet itself, it's just me being lazy for separating this model
    /// into two proper models, on for snippet to receive, and other for BuiltInMacroBase to consume for CodeGenerator
    /// instantiation. That's why this property is internal!
    /// </summary>
    internal IdAwarenessBehavior IdAwarenessBehavior { get;  set; }
    
    public Result<Func<SnippetConstruction,Type>> OverrideEntityType { get; internal set; }
    
    public RepetitionHandling RepetitionHandling { get; internal set; }

    
    public Result<Func<SnippetConstruction,string>> OverrideDbObjectName { get; internal set; }
    
    
    
}