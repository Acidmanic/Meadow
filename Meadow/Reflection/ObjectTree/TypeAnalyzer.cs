using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Meadow.Attributes;

namespace Meadow.Reflection.ObjectTree
{
    public class TypeAnalyzer
    {
        public FlatMap Map(Type type, bool fullTree = false)
        {
            var rootNode = ToAccessNode(type, fullTree);

            var leaves = rootNode.EnumerateLeavesBelow();

            if (!fullTree)
            {
                leaves = leaves.Where(l => !IsReferenceType(l.Type)).ToList();
            }

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

        public FlatMap Map<T>(bool fullTree = false)
        {
            return Map(typeof(T), fullTree);
        }

        public AccessNode ToAccessNode(Type type, bool fullTree = false)
        {
            var name = GetTableName(type);

            var root = new AccessNode(name, type);

            ToAccessNode(type, root, fullTree);

            return root;
        }

        private void ToAccessNode(Type type, AccessNode parent, bool fullTree)
        {
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;
                var refType = IsReferenceType(property.PropertyType);
                var name = refType ? GetTableName(pType) : GetMappedName(property);

                var node = new AccessNode(name, property);

                if (fullTree && refType)
                {
                    ToAccessNode(pType, node, true);
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


        public AccessNode ToAccessNode<T>(bool fullTree = false)
        {
            return ToAccessNode(typeof(T), fullTree);
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


        public string GetFieldName<TModel>(MemberExpression expression)
        {
            if (expression.Member.MemberType == MemberTypes.Property)
            {
                var counts = CountLeafMemberNames<TModel>();

                var name = expression.Member.Name;

                if (counts.ContainsKey(name) && counts[name] > 1)
                {
                    name = GetTableName(expression.Member.DeclaringType) + "." + name;
                }

                return name;
            }

            return null;
        }

        private Dictionary<string, int> CountLeafMemberNames<TModel>()
        {
            var result = new Dictionary<string, int>();

            CountLeafMemberNames(typeof(TModel), result);

            return result;
        }

        private void CountLeafMemberNames(Type type, Dictionary<string, int> result)
        {
            //TODO: cache here
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;
                var refType = IsReferenceType(property.PropertyType);
                var name = refType ? GetTableName(pType) : GetMappedName(property);

                if (refType)
                {
                    CountLeafMemberNames(pType, result);
                }
                else
                {
                    if (result.ContainsKey(name))
                    {
                        result[name]++;
                    }
                    else
                    {
                        result.Add(name, 1);
                    }
                }
            }
        }


        public TOut CreateObject<TOut>(bool fullTree)
        {
            var type = typeof(TOut);
            
            if (fullTree)
            {
                return (TOut) CreateObject(type);
            }

            return (TOut) type.GetConstructor(new Type[] { })?.Invoke(new object[] { });
        }

        private object CreateObject(Type type)
        {
            var obj = type.GetConstructor(new Type[] { })?.Invoke(new object[] { });

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;

                if (IsReferenceType(pType))
                {
                    var value = CreateObject(pType);

                    property.SetValue(obj, value);
                }
            }

            return obj;
        }
    }
}