using System;
using Meadow.Configuration;

namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets;

public class SnippetConstruction
{
    public static readonly SnippetConstruction Null = new();

    public Type EntityType { get; set; } = typeof(object);

    public MeadowConfiguration MeadowConfiguration { get; set; } = MeadowConfiguration.Null;
}