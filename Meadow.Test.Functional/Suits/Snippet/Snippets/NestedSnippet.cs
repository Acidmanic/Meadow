using Meadow.Scaffolding.Snippets;

namespace Meadow.Test.Functional.Suits.Snippet.Snippets;

public class NestedSnippet:ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;
    
    public ISnippet NextLine => new SimpleSnippet();

    public string Template => @$"
This Is My Own Code
------------------------------------------
{{{nameof(NextLine)}}}

".Trim();
    
    public static readonly string Expected = @$"
This Is My Own Code
------------------------------------------
{SimpleSnippet.Expected}

".Trim();
}