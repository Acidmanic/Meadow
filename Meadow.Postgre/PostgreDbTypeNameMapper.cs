using System;
using System.Collections.Generic;
using Meadow.DataTypeMapping;

namespace Meadow.Postgre
{
    public class PostgreDbTypeNameMapper : DbTypeNameMapperBase
    {
        private readonly Dictionary<Type, string> _typeMap = new Dictionary<Type, string>();


        public PostgreDbTypeNameMapper()
        {
            _typeMap[typeof(byte)] = "SMALLINT";
            _typeMap[typeof(short)] = "SMALLINT";
            _typeMap[typeof(ushort)] = "SMALLINT";
            _typeMap[typeof(int)] = "INT";
            _typeMap[typeof(uint)] = "INT";
            _typeMap[typeof(long)] = "BIGINT";
            _typeMap[typeof(ulong)] = "BIGINT";
            _typeMap[typeof(float)] = "float8";
            _typeMap[typeof(double)] = "float8";
            _typeMap[typeof(decimal)] = "float8";
            _typeMap[typeof(bool)] = "Boolean";
            _typeMap[typeof(string)] = "TEXT";
            _typeMap[typeof(char)] = "CHAR";
            _typeMap[typeof(Guid)] = "UUID";
            _typeMap[typeof(DateTime)] = "TIMESTAMPTZ";
            _typeMap[typeof(DateTimeOffset)] = "INTERVAL";
            _typeMap[typeof(byte[])] = "SMALLINT[]";
            _typeMap[typeof(byte?)] = "SMALLINT";
            _typeMap[typeof(sbyte?)] = "SMALLINT";
            _typeMap[typeof(short?)] = "SMALLINT";
            _typeMap[typeof(ushort?)] = "SMALLINT";
            _typeMap[typeof(int?)] = "INT";
            _typeMap[typeof(uint?)] = "INT";
            _typeMap[typeof(long?)] = "BIGINT";
            _typeMap[typeof(ulong?)] = "BIGINT";
            _typeMap[typeof(float?)] = "float8";
            _typeMap[typeof(double?)] = "float8";
            _typeMap[typeof(decimal?)] = "float8";
            _typeMap[typeof(bool?)] = "Boolean";
            _typeMap[typeof(char?)] = "CHAR";
            _typeMap[typeof(Guid?)] = "UUID";
            _typeMap[typeof(DateTime?)] = "TIMESTAMPTZ";
            _typeMap[typeof(DateTimeOffset?)] = "INTERVAL";
            //_typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;   
        }

        protected override string GetMappedType(Type type)
        {
            return _typeMap[type];
        }

        protected override string GetLargeTextDataType(Type type)
        {
            return "TEXT";
        }
    }
}