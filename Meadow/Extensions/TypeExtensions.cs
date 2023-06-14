using System;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Extensions;

public static class TypeExtensions
{
    /// <summary>
    /// If given type, is an altered type, the method will return the alternative type. Otherwise,
    /// the original type would be returned.
    /// </summary>
    public static Type GetAlteredOrOriginalType(this Type type)
    {
        var attributes = type.GetCustomAttributes<AlteredTypeAttribute>();

        var attArray = attributes as AlteredTypeAttribute[] ?? attributes.ToArray();

        if (attArray.Any())
        {
            return attArray.Last().AlternativeType;
        }

        return type;
    }
}