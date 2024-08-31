using System;
using System.Reflection;
using Acidmanic.Utilities.Results;

namespace Meadow.Extensions;


public static class TypeAttributeExtensions
{



    public static Result<TAttribute, Type> GetHierarchicalCustomAttribute<TAttribute>(this Type type) where TAttribute : Attribute
    {
        
        var parent = type;

        while (parent != null)
        {
            if (parent.GetCustomAttribute<TAttribute>() is { } attribute) return new Result<TAttribute, Type>(true,parent,attribute);

            var interfaces = parent.GetInterfaces();

            foreach (var iFace in interfaces)
            {
                if (GetHierarchicalCustomAttribute<TAttribute>(iFace) is { } iFaceAttribute) return iFaceAttribute;
            }

            parent = parent.BaseType;
        }

        return new Result<TAttribute, Type>().FailAndDefaultBothValues();
    }
}