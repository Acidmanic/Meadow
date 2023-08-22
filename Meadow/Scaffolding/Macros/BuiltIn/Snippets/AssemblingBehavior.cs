using System.Collections.Generic;
using System.Runtime.Serialization;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets;

/// <summary>
/// This class represents the snippets that a macro wants to find (searching the available assemblies) and
/// also provides configurations for each snippet.
/// </summary>
public class AssemblingBehavior : Dictionary<CommonSnippets, SnippetConfigurations>
{
    public AssemblingBehavior()
    {
    }

    public AssemblingBehavior(IDictionary<CommonSnippets, SnippetConfigurations> dictionary) : base(dictionary)
    {
    }

    public AssemblingBehavior(IDictionary<CommonSnippets, SnippetConfigurations> dictionary, IEqualityComparer<CommonSnippets> comparer) : base(dictionary, comparer)
    {
    }

    public AssemblingBehavior(IEnumerable<KeyValuePair<CommonSnippets, SnippetConfigurations>> collection) : base(collection)
    {
    }

    public AssemblingBehavior(IEnumerable<KeyValuePair<CommonSnippets, SnippetConfigurations>> collection, IEqualityComparer<CommonSnippets> comparer) : base(collection, comparer)
    {
    }

    public AssemblingBehavior(IEqualityComparer<CommonSnippets> comparer) : base(comparer)
    {
    }

    public AssemblingBehavior(int capacity) : base(capacity)
    {
    }

    public AssemblingBehavior(int capacity, IEqualityComparer<CommonSnippets> comparer) : base(capacity, comparer)
    {
    }

    protected AssemblingBehavior(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}