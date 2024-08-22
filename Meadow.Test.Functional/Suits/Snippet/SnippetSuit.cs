using Meadow.Scaffolding.Snippets;
using Meadow.Test.Functional.Suits.Snippet.Snippets;
using Xunit;

namespace Meadow.Test.Functional.Suits.Snippet;

public class SnippetSuit
{
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

    [Fact]
    public void Should_Translate_NestedSnippet()
    {
        var snippet = new NestedSnippet();

        var sut = new SnippetTranslator();

        var actual = sut.Translate(snippet);

        var expected = @"This Is My Own Code
------------------------------------------
TEST_PROPERTY1:12
TEST_PROPERTY2:My Name Is Mani";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Translate_SimpleCollection()
    {
        var snippet = new SimpleCollectionSnippet();

        var sut = new SnippetTranslator();

        var actual = sut.Translate(snippet);

        Assert.Equal(SimpleCollectionSnippet.Expected, actual);
    }
}