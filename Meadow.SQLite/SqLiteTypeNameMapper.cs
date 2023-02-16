using System;
using System.Collections.Generic;
using Meadow.DataTypeMapping;

namespace Meadow.SQLite
{
    public class SqLiteTypeNameMapper : DbTypeNameMapperBase
    {
        private readonly Dictionary<Type, string> _typeMap = new Dictionary<Type, string>();

        public SqLiteTypeNameMapper()
        {
            _typeMap[typeof(byte)] = "INTEGER";
            _typeMap[typeof(sbyte)] = "INTEGER";
            _typeMap[typeof(short)] = "INTEGER";
            _typeMap[typeof(ushort)] = "INTEGER";
            _typeMap[typeof(int)] = "INTEGER";
            _typeMap[typeof(uint)] = "INTEGER";
            _typeMap[typeof(long)] = "INTEGER";
            _typeMap[typeof(ulong)] = "INTEGER";
            _typeMap[typeof(float)] = "REAL";
            _typeMap[typeof(double)] = "REAL";
            _typeMap[typeof(decimal)] = "REAL";
            _typeMap[typeof(bool)] = "INTEGER";
            _typeMap[typeof(string)] = "TEXT";
            _typeMap[typeof(char)] = "TEXT";
            _typeMap[typeof(Guid)] = "TEXT";
            _typeMap[typeof(DateTime)] = "TEXT";
            _typeMap[typeof(DateTimeOffset)] = "TEXT";
            _typeMap[typeof(byte[])] = "BLOB";
            _typeMap[typeof(byte?)] = "INTEGER";
            _typeMap[typeof(sbyte?)] = "INTEGER";
            _typeMap[typeof(short?)] = "INTEGER";
            _typeMap[typeof(ushort?)] = "INTEGER";
            _typeMap[typeof(int?)] = "INTEGER";
            _typeMap[typeof(uint?)] = "INTEGER";
            _typeMap[typeof(long?)] = "INTEGER";
            _typeMap[typeof(ulong?)] = "INTEGER";
            _typeMap[typeof(float?)] = "REAL";
            _typeMap[typeof(double?)] = "REAL";
            _typeMap[typeof(decimal?)] = "REAL";
            _typeMap[typeof(bool?)] = "INTEGER";
            _typeMap[typeof(char?)] = "TEXT";
            _typeMap[typeof(Guid?)] = "TEXT";
            _typeMap[typeof(DateTime?)] = "TEXT";
            _typeMap[typeof(DateTimeOffset?)] = "TEXT";
            //_typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;   
        }

        public override string GetDatabaseTypeName(Type type)
        {
            return _typeMap[type];
        }
    }
}