using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Meadow.Attributes;

namespace Meadow.Reflection
{
    public class TypeFieldMapHelper
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

        private void ListTypesInvolved(List<Type> result, Type type)
        {
            result.Add(type);

            var properties = type.GetProperties();

            foreach (var prop in properties)
            {
                var propType = prop.PropertyType;

                if (IsReferenceType(propType))
                {
                    ListTypesInvolved(result, propType);
                }
            }
        }

        public List<Type> ListTypesInvolved(Type type)
        {
            var result = new List<Type>();

            ListTypesInvolved(result, type);

            return result;
        }


        public bool IsReferenceType(Type t)
        {
            return !t.IsPrimitive &&
                   !t.IsValueType &&
                   t != typeof(string) &&
                   t != typeof(char);
        }

        public GroupPropertyMap MapProperties(Type type)
        {
            var result = new GroupPropertyMap();

            var typesInvolved = ListTypesInvolved(type);

            var fieldsPerType = new Dictionary<Type, List<string>>();

            var counts = new Dictionary<string, int>();

            foreach (var typeInvolved in typesInvolved)
            {
                List<string> fieldNames = GetAllFieldNames(typeInvolved);

                fieldsPerType.Add(typeInvolved, fieldNames);

                foreach (var name in fieldNames)
                {
                    if (counts.ContainsKey(name))
                    {
                        counts[name]++;
                    }
                    else
                    {
                        counts.Add(name, 1);
                    }
                }
            }

            foreach (var item in fieldsPerType)
            {
                var iType = item.Key;

                var fields = item.Value;

                var respectives = new List<string>();

                foreach (var fieldName in fields)
                {
                    if (counts[fieldName] == 1)
                    {
                        respectives.Add(fieldName);
                    }
                }

                result.Respectives.Add(iType, respectives);
            }

            result.Common.AddRange(
                counts.Where(item => item.Value > 1)
                    .Select(item => item.Key)
            );
            return result;
        }

        private List<string> GetAllFieldNames(Type type)
        {
            var properties = type.GetProperties();

            var result = new List<string>();

            foreach (var property in properties)
            {
                var propType = property.PropertyType;

                if (!IsReferenceType(propType))
                {
                    result.Add(property.Name);
                }
            }

            return result;
        }
    }
}