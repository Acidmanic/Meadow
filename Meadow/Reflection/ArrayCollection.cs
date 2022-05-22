using System;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Reflection
{
    public class ArrayCollection : ICollection<object>
    {
        private Array _array;
        private readonly Type _elementType;

        public ArrayCollection(Array array)
        {
            _array = array;

            _elementType = array.GetType().GetElementType();
        }

        public ArrayCollection(Type elementType)
        {
            _array = Array.CreateInstance(elementType, 0);

            _elementType = elementType;
        }


        private class ObjectEnumerator : IEnumerator<Object>
        {
            private readonly IEnumerator _enumerator;

            public ObjectEnumerator(Array array)
            {
                _enumerator = array.GetEnumerator();
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public object Current => _enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return new ObjectEnumerator(_array);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(object value)
        {
            var newArray = Array.CreateInstance(_elementType, _array.Length + 1);

            Array.Copy(_array, newArray, _array.Length);

            newArray.SetValue(value, _array.Length);

            _array = newArray;
        }

        public void Clear()
        {
            _array = Array.CreateInstance(_elementType, 0);
        }

        public bool Contains(object item)
        {
            foreach (var element in _array)
            {
                if (element == item)
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            int remaining = array.Length - arrayIndex;

            int length = remaining < _array.Length ? remaining : _array.Length;

            for (int i = 0; i < length; i++)
            {
                array[i + arrayIndex] = _array.GetValue(i);
            }
        }

        public bool Remove(object item)
        {
            throw new NotImplementedException();
        }

        public int Count => _array.Length;
        public bool IsReadOnly => false;

        public Array WrappedArray => _array;
    }
}