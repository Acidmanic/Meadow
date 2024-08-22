using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Scaffolding.Snippets;

public interface ISnippet
{

    [IgnoreNoneDataNode]
    SnippetToolbox? Toolbox { get; set; }
    
    
    string Template { get; }
}