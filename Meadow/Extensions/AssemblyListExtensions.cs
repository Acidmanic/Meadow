using System;
using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;

namespace Meadow.Extensions;

public static class AssemblyListExtensions
{
    
    public static List<Type> ListAllAvailableClasses(this IEnumerable<Assembly> assemblies)
    {
        var allAvailableClasses = new List<Type>();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetAvailableTypes();

            allAvailableClasses.AddRange(types);
        }

        return allAvailableClasses;
    }
}