using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets;

public class AssemblingBehaviorBuilder
{
    private readonly AssemblingBehavior _assemblyBehavior = new AssemblingBehavior();


    public AssemblingBehavior Build()
    {
        return new AssemblingBehavior(_assemblyBehavior);
    }


    public SnippetConfigurationBuilder Add(CommonSnippets snippet)
    {
        var currentConfigurations = new SnippetConfigurations();

        _assemblyBehavior.Add(snippet, currentConfigurations);

        var builder = new SnippetConfigurationBuilder(currentConfigurations);

        return builder;
    }
}