using System.Collections.Generic;
using Meadow.Scaffolding.Snippets;

namespace Meadow.Test.Functional.Suits.Snippet.Snippets;

public class SimpleCollectionSnippet : ISnippet
{
    public SnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;


    public List<string> Collection => new()
    {
        "A", "B", "C", "D"
    };

    public string Template => "{Collection}";


    public static readonly string Expected
        = @"A
B
C
D
";
}