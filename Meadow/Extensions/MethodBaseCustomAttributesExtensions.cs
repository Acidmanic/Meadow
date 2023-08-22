using System;
using System.Collections.Generic;
using System.Reflection;

namespace Meadow.Extensions;

public static class MethodBaseCustomAttributesExtensions
{
    public static List<T> GetCustomAttributes<T>(this MethodBase method) where T : Attribute
    {
        var customAttributes = method.GetCustomAttributes();

        var attributesList = new List<T>();
        
        foreach (var customAttribute in customAttributes)
        {
            if (customAttribute is T t)
            {
                attributesList.Add(t);
            }    
        }

        return attributesList;
    }
}