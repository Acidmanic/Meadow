using System;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Reflection
{
    public static class TypeCheck
    {
        public static bool IsCollection(Type type)
        {
            return Implements<ICollection>(type);
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
    }
}