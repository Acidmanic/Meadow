using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Meadow.Reflection.ObjectTree;
using Meadow.Reflection.ObjectTree.DataSource;

namespace Meadow.Test.Functional.TestDoubles
{
    public class InMemoryDataReader : IDataReader
    {
        private class Record
        {
            public List<DataPoint> Fields { get; }

            public Dictionary<string, DataPoint> FieldsByName { get; }
            public Dictionary<string, int> Ordinals { get; }

            public Record()
            {
                Fields = new List<DataPoint>();
                FieldsByName = new Dictionary<string, DataPoint>();
                Ordinals = new Dictionary<string, int>();
            }

            public void InsertField(string fieldName, object value)
            {
                var data = new DataPoint
                {
                    Identifier = fieldName,
                    Value = value
                };
                Ordinals.Add(fieldName, Fields.Count);
                Fields.Add(data);
                FieldsByName.Add(fieldName, data);
            }

            public string GetName(int index)
            {
                return Fields[index].Identifier;
            }

            public Type GetType(int index)
            {
                return Fields[index].Value?.GetType();
            }

            public object GetValue(int index)
            {
                if (Fields.Count > index)
                {
                    return Fields[index].Value;
                }

                return null;
            }

            public object GetValue(string fieldName)
            {
                if (FieldsByName.ContainsKey(fieldName))
                {
                    return FieldsByName[fieldName].Value;
                }

                return null;
            }

            public int GetOrdinal(string fieldName)
            {
                if (Ordinals.ContainsKey(fieldName))
                {
                    return Ordinals[fieldName];
                }

                return -1;
            }
        }

        private readonly List<Record> _records;
        private int _recordIndex;

        private Record HeaderInfo
        {
            get
            {
                var record = _records[0];

                for (int i = 1; i < _records.Count; i++)
                {
                    if (_records[i].Fields.Count > record.Fields.Count)
                    {
                        record = _records[i];
                    }
                }

                return record;
            }
        }


        public InMemoryDataReader()
        {
            _records = new List<Record>();
            _recordIndex = -1;
        }

        public InMemoryDataReader CreateRecord()
        {
            _records.Add(new Record());
            return this;
        }

        private Record LastRecord()
        {
            if (_records.Count == 0)
            {
                CreateRecord();
            }

            return _records.Last();
        }

        private Record CurrentRecord => _records[_recordIndex];

        public InMemoryDataReader InsertField(string fieldName, object value)
        {
            LastRecord().InsertField(fieldName, value);

            return this;
        }


        private T CastOrDefault<T>(object value)
        {
            if (value is T tValue)
            {
                return tValue;
            }

            return default;
        }

        public bool GetBoolean(int i)
        {
            return CastOrDefault<bool>(CurrentRecord.GetValue(i));
        }

        public byte GetByte(int i)
        {
            return CastOrDefault<byte>(CurrentRecord.GetValue(i));
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return CastOrDefault<char>(CurrentRecord.GetValue(i));
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            return HeaderInfo.GetType(i).Name;
        }

        public DateTime GetDateTime(int i)
        {
            return CastOrDefault<DateTime>(CurrentRecord.GetValue(i));
        }

        public decimal GetDecimal(int i)
        {
            return CastOrDefault<decimal>(CurrentRecord.GetValue(i));
        }

        public double GetDouble(int i)
        {
            return CastOrDefault<double>(CurrentRecord.GetValue(i));
        }

        public Type GetFieldType(int i)
        {
            return HeaderInfo.GetType(i);
        }

        public float GetFloat(int i)
        {
            return CastOrDefault<float>(CurrentRecord.GetValue(i));
        }

        public Guid GetGuid(int i)
        {
            return CastOrDefault<Guid>(CurrentRecord.GetValue(i));
        }

        public short GetInt16(int i)
        {
            return CastOrDefault<short>(CurrentRecord.GetValue(i));
        }

        public int GetInt32(int i)
        {
            return CastOrDefault<int>(CurrentRecord.GetValue(i));
        }

        public long GetInt64(int i)
        {
            return CastOrDefault<long>(CurrentRecord.GetValue(i));
        }

        public string GetName(int i)
        {
            return HeaderInfo.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return HeaderInfo.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return CastOrDefault<string>(CurrentRecord.GetValue(i));
        }

        public object GetValue(int i)
        {
            return CurrentRecord.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return CurrentRecord.GetValue(i) == null;
        }

        public int FieldCount => HeaderInfo.Fields.Count;

        public object this[int i] => CurrentRecord.GetValue(i);

        public object this[string name] => CurrentRecord.GetValue(name);

        public void Dispose()
        {
        }

        public void Close()
        {
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            if (_recordIndex < _records.Count - 1)
            {
                _recordIndex++;
                return true;
            }

            return false;
        }

        public bool Read()
        {
            return NextResult();
        }

        public int Depth => 0;
        public bool IsClosed => false;
        public int RecordsAffected => _records.Count;
    }
}