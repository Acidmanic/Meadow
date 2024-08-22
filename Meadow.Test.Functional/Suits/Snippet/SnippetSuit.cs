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

        Assert.Equal(SimpleSnippet.Expected, actual);
    }

    [Fact]
    public void Should_Translate_NestedSnippet()
    {
        var snippet = new NestedSnippet();

        var sut = new SnippetTranslator();

        var actual = sut.Translate(snippet);

        Assert.Equal(NestedSnippet.Expected, actual);
    }

    [Fact]
    public void Should_Translate_SimpleCollection()
    {
        var snippet = new SimpleCollectionSnippet();

        var sut = new SnippetTranslator();

        var actual = sut.Translate(snippet);

        Assert.Equal(SimpleCollectionSnippet.Expected, actual);
    }
    
    [Fact]
    public void Should_Translate_NestedCollection()
    {
        var snippet = new NestedCollectionSnippet();

        var sut = new SnippetTranslator();

        var actual = sut.Translate(snippet);

        Assert.Equal(NestedCollectionSnippet.Expected, actual);
    }
}