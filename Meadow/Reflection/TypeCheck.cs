using System;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Reflection
{
    public static class TypeCheck
    {
        private static Dictionary<Type, bool> _isModelCache = new Dictionary<Type, bool>();

        public static bool IsCollection(Type type)
        {
            return Implements<ICollection>(type);
        }

        public static bool InheritsFrom<TSuper>(Type type)
        {
            return Extends<TSuper>(type) || Implements<TSuper>(type);
        }

        public static bool Implements<TInterface>(Type type)
        {
            var parent = type;

            var checkingType = typeof(TInterface);

            while (parent != null)
            {
                var allInterfaces = type.GetInterfaces();

                foreach (var i in allInterfaces)
                {
                    if (i == checkingType)
                    {
                        return true;
                    }
                }

                parent = parent.DeclaringType;
            }

            return false;
        }

        public static bool Extends<TSuper>(Type type)
        {
            var parent = type;

            var checkingType = typeof(TSuper);

            while (parent != null)
            {
                if (parent == checkingType)
                {
                    return true;
                }

                parent = parent.DeclaringType;
            }

            return false;
        }

        public static bool IsReferenceType(Type t)
        {
            return !t.IsPrimitive &&
                   !t.IsValueType &&
                   t != typeof(string) &&
                   t != typeof(char);
        }

        public static List<Type> EnumerateEntities(Type type)
        {
            var result = new List<Type>();

            EnumerateEntities(type, result);

            return result;
        }

        private static void EnumerateEntities(Type type, List<Type> result)
        {
            if (IsReferenceType(type))
            {
                if (IsCollection(type))
                {
                    type = type.GenericTypeArguments[0];
                }

                result.Add(type);
            }

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;

                EnumerateEntities(pType, result);
            }
        }

        public static bool IsModel(Type type)
        {
            if (_isModelCache.ContainsKey(type))
            {
                return _isModelCache[type];
            }

            var isModel = IsModelNoneCached(type);

            _isModelCache.Add(type, isModel);

            return isModel;
        }

        private static bool IsModelNoneCached(Type type)
        {
            if (type.IsAbstract || type.IsInterface || type.IsGenericType)
            {
                return false;
            }

            if (!IsReferenceType(type))
            {
                return false;
            }

            var properties = type.GetProperties();

            if (properties.Length == 0)
            {
                return false;
            }

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                {
                    return false;
                }

                var pType = property.PropertyType;

                if (IsReferenceType(pType) && !(IsModel(pType) || IsCollection(pType)))
                {
                    return false;
                }
            }

            return true;
        }

        public static Type GetElementType(Type type)
        {
            if (IsCollection(type))
            {
                if (type.IsArray)
                {
                    return type.GetElementType();
                }
                else if (type.GenericTypeArguments.Length > 0)
                {
                    return type.GenericTypeArguments[0];
                }

                return typeof(object);
            }

            return null;
        }
    }
}