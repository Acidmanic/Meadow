using System.Collections.Generic;
using Meadow.Scaffolding.Snippets;

namespace Meadow.Test.Functional.Suits.Snippet.Snippets;

public class NestedCollectionSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;


    public List<ISnippet> Collection => new()
    {
        new SimpleSnippet(),
        new SimpleSnippet(),
    };

    public string Template => "{Collection}";


    public static readonly string Expected
        = $"{SimpleSnippet.Expected}\n{SimpleSnippet.Expected}\n";
}