using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Meadow.Attributes;

namespace Meadow.Reflection.ObjectTree
{
    public class TypeAnalyzer
    {
        public FlatMap Map(Type type)
        {
            var rootNode = ToAccessNode(type);

            var leaves = rootNode.EnumerateLeavesBelow();

            var counts = CountFieldNames(leaves);

            var map = new FlatMap();

            foreach (var leaf in leaves)
            {
                var name = leaf.Name;

                if (counts[name] > 1)
                {
                    name = leaf.Parent.Name + "." + name;
                }
                map.Add(name,
                    o => leaf.GetValue(o),
                    (o, v) => leaf.SetValue(o, v)
                );
            }

            return map;
        }

        public FlatMap Map<T>()
        {
            return Map(typeof(T));
        }

        public AccessNode ToAccessNode(Type type)
        {
            var name = GetTableName(type);

            var root = new AccessNode(name, type);

            ToAccessNode(type, root);

            return root;
        }

        private void ToAccessNode(Type type, AccessNode parent)
        {
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;
                var refType = IsReferenceType(property.PropertyType);
                var name = refType ? GetTableName(pType) : GetMappedName(property);

                var node = new AccessNode(name, property);

                if (refType)
                {
                    ToAccessNode(pType, node);
                }

                parent.Add(node);
            }
        }

        public string GetMappedName(PropertyInfo property)
        {
            string name = property.Name;
            //TODO: Put naming conventions here   

            List<FieldAttribute> attributes = new List<FieldAttribute>();

            attributes.AddRange(property.GetCustomAttributes<ColumnAttribute>());

            if (attributes.Count > 0)
            {
                name = attributes.Last().Name;
            }

            return name;
        }

        private string GetTableName(Type pType)
        {
            var attributes = pType.GetCustomAttributes<TableAttribute>().ToList();

            if (attributes.Count > 0)
            {
                return attributes.Last().TableName;
            }

            return pType.Name + "s";
        }


        public AccessNode ToAccessNode<T>()
        {
            return ToAccessNode(typeof(T));
        }


        public bool IsReferenceType(Type t)
        {
            return !t.IsPrimitive &&
                   !t.IsValueType &&
                   t != typeof(string) &&
                   t != typeof(char);
        }


        public Dictionary<string, int> CountFieldNames(List<AccessNode> nodes)
        {
            Dictionary<string, int> fieldCount = new Dictionary<string, int>();

            foreach (var node in nodes)
            {
                var field = node.Name;

                if (fieldCount.ContainsKey(field))
                {
                    fieldCount[field] += 1;
                }
                else
                {
                    fieldCount.Add(field, 1);
                }
            }

            return fieldCount;
        }
    }
}