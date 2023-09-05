using System;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Meadow.Search.Models;

namespace Meadow.Search.Utilities;

public static class FilteringTypeUtilities
{


    public static Type GetFilterResultsType(Type entityType)
    {
        return MakeSpecifiedTypeForGenericIdType(typeof(FilterResult<>), entityType);
    }
    
    public static Type GetSearchIndexType(Type entityType)
    {
        return MakeSpecifiedTypeForGenericIdType(typeof(SearchIndex<>), entityType);
    }
    
    private static Type MakeSpecifiedTypeForGenericIdType(Type idGenericType, Type entityType)
    {
        var idLeaf = TypeIdentity.FindIdentityLeaf(entityType);

        if (idLeaf == null)
        {
            throw new Exception($"WARNING: the entity of type {entityType.FullName}" +
                                $", does not have an identifier field. There for it's not possible " +
                                $"to have a {idGenericType.Name} table created for it.");
        }

        var genericType = idGenericType;

        var specifiedType = genericType.MakeGenericType(idLeaf.Type);
            
        return specifiedType;
    }
}