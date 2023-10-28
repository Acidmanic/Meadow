using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Meadow.DataTypeMapping.Attributes;

namespace Meadow.DataTypeMapping;

public abstract class DbTypeNameMapperBase : IDbTypeNameMapper
{
    protected string OverrideSizeInParentheses(string typeName, long size)
    {
        var rx = new Regex("\\(\\s*.+\\s*\\)");

        var sizeString = $"({size.ToString()})";

        typeName = rx.Replace(typeName, sizeString, 1);

        return typeName;
    }

  
    
    public abstract string GetDatabaseTypeName(Type type);

    protected abstract string GetLargeTextDataType();

    public string GetDatabaseTypeName(Type type, IEnumerable<Attribute> propertyAttributes)
    {

        if (type == typeof(string))
        {
            var isLargeText = propertyAttributes.Any(a => a is IsLargeTextAttribute);

            if (isLargeText)
            {
                return GetLargeTextDataType();
            }
        }
        
        var forcedTypeAttribute = propertyAttributes
            .OfType<ForceDatabaseTypeAttribute>().FirstOrDefault();

        if (forcedTypeAttribute != null)
        {
            return forcedTypeAttribute.DatabaseTypeName;
        }

        var passingType = type.IsEnum ? typeof(int) : type;

        var sizeAttribute = propertyAttributes
            .OfType<ForceColumnSizeAttribute>().FirstOrDefault();

        var databaseTypeName = GetDatabaseTypeName(passingType);

        if (sizeAttribute != null)
        {
            databaseTypeName = AdjustSize(passingType, databaseTypeName, sizeAttribute.ColumnSize);
        }

        databaseTypeName = PostUpdateMappedType(passingType, databaseTypeName, propertyAttributes);

        return databaseTypeName;
    }

    protected virtual string AdjustSize(Type type, string databaseTypeName, long size)
    {
        return databaseTypeName;
    }

    protected virtual string PostUpdateMappedType(Type type,
        string databaseTypeName,
        IEnumerable<Attribute> propertyAttributes)
    {
        return databaseTypeName;
    }
}