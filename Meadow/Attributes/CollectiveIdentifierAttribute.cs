using System;
using System.Collections.Generic;

namespace Meadow.Attributes;

/// <summary>
/// In Soma cases, A collection Of Fields of a model, all together can identify the instance.
/// These fields can be marked with this attribute. This metadata can be used in Save operation
/// to create where expression 
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class CollectiveIdentifierAttribute : Attribute
{

    
    
    public CollectiveIdentifierAttribute(params string[] collectionNames)
    {
        if (collectionNames.Length == 0)
        {
            collectionNames = new string[] { "Identifiers" };
        }

        CollectionNames = collectionNames;
    }

    public string[] CollectionNames { get; }
}