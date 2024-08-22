using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Scaffolding.Snippets;
using Xunit;

namespace Meadow.Test.Functional.Suits;

public class SnippetSuit
{
    public class SimpleSnippet : ISnippet
    {
        public int Property1 => 12;

        public string Property2 => "My Name Is Mani";

        [IgnoreNoneDataNode] public SnippetToolbox? Toolbox { get; set; }


        public string Template => @"
TEST_PROPERTY1:{Property1}
TEST_PROPERTY2:{Property2}
".Trim();
    }


    [Fact]
    public void Should_Translate_SimpleSnippet()
    {
        var snippet = new SimpleSnippet();

        var sut = new SnippetTranslator();

        var actual = sut.Translate(snippet);

        var expected = @"TEST_PROPERTY1:12
TEST_PROPERTY2:My Name Is Mani";

        Assert.Equal(expected, actual);
    }
}