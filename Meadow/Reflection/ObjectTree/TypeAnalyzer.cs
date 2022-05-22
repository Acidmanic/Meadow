using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Meadow.Attributes;
using Meadow.Reflection.Conventions;

namespace Meadow.Reflection.ObjectTree
{
    public class TypeAnalyzer
    {
        
        public ITableNameProvider TableNameProvider { get; set; } = new PluralTableNameProvider();
        
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

                map.Add(name, o => leaf.GetValue(o), (o, v) => leaf.SetValue(o, v));
            }

            return map;
        }

        public FlatMap Map<T>(bool fullTree = false)
        {
            return Map(typeof(T), fullTree);
        }

        public AccessNode ToAccessNode(Type type, bool fullTree = false)
        {
            var name = TableNameProvider.GetTableName(type);

            var root = new AccessNode(name, type);

            ToAccessNode(type, root, fullTree);

            return root;
        }

        private void ToAccessNode(Type type, AccessNode parent, bool fullTree, PropertyInfo parentPropertyInfo = null)
        {
            var isCollection = IsCollection(type);
            var elementType = isCollection ? type.GenericTypeArguments[0] : null;

            var properties = isCollection ? elementType.GetProperties() : type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;

                var refType = IsReferenceType(pType);

                var name = IsCollection(pType) ? TableNameProvider.GetTableName(pType.GenericTypeArguments[0]) :
                    refType ? TableNameProvider.GetTableName(pType) : GetMappedName(property);

                var node = isCollection
                    ? new CollectionAccessNode(name, parentPropertyInfo, property, elementType)
                    : new AccessNode(name, property);

                if (fullTree && refType)
                {
                    ToAccessNode(pType, node, true, property);
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
                    name = TableNameProvider.GetTableName(expression.Member.DeclaringType) + "." + name;
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
                var name = refType ? TableNameProvider.GetTableName(pType) : GetMappedName(property);

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

            return (TOut) BlindInstantiate(type);
        }


        public object BlindInstantiate(Type type)
        {
            return type.GetConstructor(new Type[] { })?.Invoke(new object[] { });
        }

        private object CreateObject(Type type)
        {
            var obj = BlindInstantiate(type);

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;

                if (IsReferenceType(pType))
                {
                    object value;

                    if (IsCollection(pType))
                    {
                        value = BlindInstantiate(pType);
                    }
                    else if (pType.IsArray)
                    {
                        value = Array.CreateInstance(pType.GetElementType() ?? typeof(Object), 0);
                    }
                    else
                    {
                        value = CreateObject(pType);
                    }

                    property.SetValue(obj, value);
                }
            }

            return obj;
        }

        private bool IsCollection(Type type)
        {
            var parent = type;

            while (parent != null)
            {
                var allInterfaces = type.GetInterfaces();

                foreach (var i in allInterfaces)
                {
                    if (i == typeof(ICollection))
                    {
                        return true;
                    }
                }

                parent = parent.DeclaringType;
            }

            return false;
        }
    }
}