using Meadow.Scaffolding.Snippets;

namespace Meadow.Test.Functional.Suits.Snippet.Snippets;

public class NestedSnippet:ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }
    
    public ISnippet NextLine => new SimpleSnippet();

    public string Template => @$"
This Is My Own Code
------------------------------------------
{{{nameof(NextLine)}}}

".Trim();
}