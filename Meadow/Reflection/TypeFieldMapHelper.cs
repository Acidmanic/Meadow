using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Meadow.Attributes;

namespace Meadow.Reflection
{
    class TypeFieldMapHelper
    {
        public Dictionary<string, Accessor> GetTableMap<T>(FieldNameType fieldNameType)
        {
            var map = new Dictionary<string, Accessor>();

            var type = typeof(T);

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                string mappedName = GetMappedName(property, fieldNameType);

                var accessor = new Accessor();

                accessor.Getter = obj => property.GetValue(obj);

                accessor.Setter = (obj, value) => property.SetValue(obj, value);

                map.Add(mappedName, accessor);
            }

            //TODO: Here you can cache a map of string, func<object,object> for 
            //TODO: each type, where the second object is the storage itself to be passed
            return map;
        }

        public string GetMappedName(PropertyInfo property, FieldNameType fieldNameType)
        {
            string name = property.Name;
            //TODO: Put naming conventions here   

            List<FieldAttribute> attributes = new List<FieldAttribute>();

            if (fieldNameType == FieldNameType.ColumnName)
            {
                attributes.AddRange(property.GetCustomAttributes<ColumnAttribute>());
            }
            else
            {
                attributes.AddRange(property.GetCustomAttributes<ParameterAttribute>());
            }

            if (attributes.Count > 0)
            {
                name = attributes.Last().Name;
            }

            return name;
        }
    }
}