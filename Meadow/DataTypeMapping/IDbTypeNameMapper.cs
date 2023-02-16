using System;
using System.Collections.Generic;

namespace Meadow.DataTypeMapping
{
    public interface IDbTypeNameMapper
    {
        
        string GetDatabaseTypeName(Type type);
        /// <summary>
        /// If The Database would not be affected by any attributes, just forward the other method.
        /// </summary>
        string GetDatabaseTypeName(Type type,IEnumerable<Attribute> propertyAttributes);
    }
}