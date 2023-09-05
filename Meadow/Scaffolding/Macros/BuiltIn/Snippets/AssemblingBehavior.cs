using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets;

/// <summary>
/// This class represents the snippets that a macro wants to find (searching the available assemblies) and
/// also provides configurations for each snippet.
/// </summary>
public class AssemblingBehavior : List<SnippetOrder>
{
    public AssemblingBehavior()
    {
    }

    public AssemblingBehavior(IEnumerable<SnippetOrder> collection) : base(collection)
    {
    }


    public void Add(CommonSnippets snippet, SnippetConfigurations configurations)
    {
        Add(new SnippetOrder
        {
            Snippet = snippet,
            Configurations = configurations
        });
    }
}