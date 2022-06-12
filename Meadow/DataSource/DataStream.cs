using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Meadow.DataSource
{
    public class DataStream : IDataStream
    {
        private readonly IDataReader _dataReader;

        public DataStream(IDataReader dataReader)
        {
            _dataReader = dataReader;
        }


        private class DataPointEnumerator : IEnumerator<DataPoint>
        {
            private readonly IDataReader _dataReader;
            private readonly List<string> _fields;
            private int _fieldIndex;


            public DataPointEnumerator(IDataReader dataReader)
            {
                _dataReader = dataReader;
                _fields = EnumFields(dataReader);
                _fieldIndex = 0;
            }


            public bool MoveNext()
            {
                if (_fieldIndex >= _fields.Count)
                {
                    return _dataReader.NextResult();
                }

                _fieldIndex++;
                return true;
            }

            public void Reset()
            {
                _fieldIndex = 0;
            }

            public DataPoint Current
            {
                get
                {
                    var fieldName = _fields[_fieldIndex];

                    var value = _dataReader[fieldName];

                    return new DataPoint
                    {
                        Identifier = fieldName,
                        Value = value
                    };
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            private List<string> EnumFields(IDataReader dataReader)
            {
                var result = new List<string>();

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    result.Add(dataReader.GetName(i));
                }

                return result;
            }
        }


        public IEnumerator<DataPoint> GetEnumerator()
        {
            return new DataPointEnumerator(_dataReader);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}