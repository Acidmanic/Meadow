using System;
using Meadow.Scaffolding.Macros.BuiltIn;

namespace Meadow.Scaffolding.Attributes;

public class CommonSnippetAttribute : Attribute
{
    public CommonSnippetAttribute(CommonSnippets snippetType)
    {
        SnippetType = snippetType;
    }

    public CommonSnippets SnippetType { get; }
}