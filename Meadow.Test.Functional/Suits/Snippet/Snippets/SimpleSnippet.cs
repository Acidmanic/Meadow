using Meadow.Scaffolding.Snippets;

namespace Meadow.Test.Functional.Suits.Snippet.Snippets;

public class SimpleSnippet : ISnippet
{
    public int Property1 => 12;

    public string Property2 => "My Name Is Mani";

    public SnippetToolbox? Toolbox { get; set; }


    public string Template => @"
TEST_PROPERTY1:{Property1}
TEST_PROPERTY2:{Property2}
".Trim();
}