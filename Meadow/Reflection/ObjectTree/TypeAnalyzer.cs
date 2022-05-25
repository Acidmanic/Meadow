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
        
        public AccessNode ToAccessNode(Type type, bool fullTree = false)
        {
            return ToAccessNode(type, fullTree, null, 0);
        }

        /// <summary>
        /// The Main AccessNode Creation method 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fullTree"></param>
        /// <param name="callerProperty"></param>
        /// <returns></returns>
        private AccessNode ToAccessNode(Type type, bool fullTree, PropertyInfo callerProperty, int depth)
        {
            var isCollection = TypeCheck.IsCollection(type);

            var isReference = TypeCheck.IsReferenceType(type);

            var isUnique = IsUnique(callerProperty);

            var elementType = isCollection ? type.GenericTypeArguments[0] : null;

            var elementTableName = elementType == null ? null : TableNameProvider.GetTableName(elementType);

            var name = isCollection ? "<" + elementType + ">" :
                isReference ? TableNameProvider.GetTableName(type) : GetMappedName(callerProperty);

            var node = new AccessNode(name, type, callerProperty, isUnique, depth);

            if (isCollection)
            {
                var collectionChild = ToAccessNode(elementType, fullTree, null, depth + 1);

                node.Add(collectionChild);
            }
            else if (isReference)
            {
                var properties = type.GetProperties();

                foreach (var property in properties)
                {
                    var pType = property.PropertyType;

                    if (!TypeCheck.IsReferenceType(pType) || fullTree)
                    {
                        var child = ToAccessNode(pType, fullTree, property, depth + 1);

                        node.Add(child);
                    }
                }
            }

            return node;
        }

        private bool IsUnique(PropertyInfo property)
        {
            if (property == null)
            {
                return false;
            }

            var attributes = property.GetCustomAttributes<UniqueFieldAttribute>();

            return attributes.Any();
        }

        public string GetMappedName(PropertyInfo property)
        {
            string name = property.Name;
            //TODO: Put naming conventions here   

            List<FieldAttribute> attributes = new List<FieldAttribute>();

            attributes.AddRange(property.GetCustomAttributes<FieldAttribute>());

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
                var refType = TypeCheck.IsReferenceType(property.PropertyType);
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

        public object CreateObject(Type type)
        {
            var obj = BlindInstantiate(type);

            if (TypeCheck.IsCollection(type))
            {
                return obj;
            }

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;

                if (TypeCheck.IsReferenceType(pType))
                {
                    object value;

                    if (TypeCheck.IsCollection(pType))
                    {
                        value = BlindInstantiate(pType);
                    }
                    else if (pType.IsArray)
                    {
                        value = Array.CreateInstance(pType.GetElementType() ?? typeof(object), 0);
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
    }
}