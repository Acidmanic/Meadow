using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Scaffolding.Snippets;

public interface ISnippet
{

    [IgnoreNoneDataNode]
    ISnippetToolbox Toolbox { get; set; }
    
    [IgnoreNoneDataNode]
    string Template { get; }
}