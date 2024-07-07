using System;
using System.Reflection;

namespace Meadow.Test.Functional.Utility
{


    internal static class PropertyWrapper
    {
        public static PropertyWrapper<object> Create(string propertyName, Type propertyType, object obj)
        {
            return Create<object>(propertyName, propertyType, obj);
        }

        public static PropertyWrapper<TProperty> Create<TProperty>(string propertyName, object obj)
        {
            return Create<TProperty>(propertyName, typeof(TProperty), obj);
        }

        private static PropertyWrapper<TProperty> Create<TProperty>(string propertyName, Type propertyType, object obj)
        {
            if (obj != null)
            {
                var oType = obj.GetType();

                var properties = oType.GetProperties();

                foreach (var property in properties)
                {
                    if (property.Name == propertyName && property.PropertyType == propertyType)
                    {
                        return new PropertyWrapper<TProperty>(property, obj);
                    }
                }
            }

            return new PropertyWrapper<TProperty>(
                () => default,
                o => { }
            );
        }
    }
    
    internal class PropertyWrapper<T>
    {
        private Func<T> _read = () => default;
        private Action<T> _write = (value) => { };


        public T Value
        {
            get { return _read(); }
            set { _write(value); }
        }
        
        public PropertyWrapper(PropertyInfo propertyInfo, object obj)
        {
            if (propertyInfo.CanRead)
            {
                _read = () =>
                {
                    try
                    {
                        return (T) propertyInfo.GetValue(obj);
                    }
                    catch (Exception)
                    {
                    }

                    return default;
                };
            }

            if (propertyInfo.CanWrite)
            {
                _write = (value) =>
                {
                    try
                    {
                        propertyInfo.SetValue(obj, (object) value);
                    }
                    catch (Exception)
                    {
                    }
                };
            }
        }

        public PropertyWrapper(Func<T> read, Action<T> write)
        {
            _read = read;

            _write = write;
        }
    }
}