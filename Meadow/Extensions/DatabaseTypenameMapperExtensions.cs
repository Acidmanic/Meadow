using System;
using Meadow.DataTypeMapping;
using Meadow.DataTypeMapping.Attributes;

namespace Meadow.Extensions;

public static class DatabaseTypenameMapperExtensions
{
    public static string GetDatabaseTypeName(this IDbTypeNameMapper mapper, Type type, long columnSize)
    {
        return mapper.GetDatabaseTypeName(type, new[] { new ForceColumnSizeAttribute(columnSize) });
    }


    public static string GetDatabaseTypeNameForString(this IDbTypeNameMapper mapper, long columnSize)
    {
        return mapper.GetDatabaseTypeName(typeof(string), columnSize);
    }
}