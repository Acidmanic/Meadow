using System;
using System.Collections.Generic;

namespace Meadow.DataTypeMapping
{
    public interface IDbTypeNameMapper
    {
        public static readonly IDbTypeNameMapper Null = new NullDbNameMapper();

        string GetDatabaseTypeName(Type type);
        /// <summary>
        /// If The Database would not be affected by any attributes, just forward the other method.
        /// </summary>
        string GetDatabaseTypeName(Type type,IEnumerable<Attribute> propertyAttributes);
        
        private class NullDbNameMapper :IDbTypeNameMapper
        {
            public string GetDatabaseTypeName(Type type) => type.Name;

            public string GetDatabaseTypeName(Type type, IEnumerable<Attribute> _) => GetDatabaseTypeName(type);

        }
        
    }
    
    
    
}