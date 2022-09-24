using System;
using System.Collections.Generic;
using System.Data;
using Meadow.DataTypeMapping;

namespace Meadow.Postgre
{
    public class PostgreDbTypeNameMapper : IDbTypeNameMapper
    {
        private readonly Dictionary<Type, string> _typeMap = new Dictionary<Type, string>();

        public string this[Type type]
        {
            get
            {
                if (type.IsEnum)
                {
                    return _typeMap[typeof(int)];
                }

                return _typeMap[type];
            }
        }

        public PostgreDbTypeNameMapper()
        {
            _typeMap[typeof(byte)] = "SMALLINT";
            _typeMap[typeof(short)] = "SMALLINT";
            _typeMap[typeof(ushort)] = "SMALLINT";
            _typeMap[typeof(int)] = "INT";
            _typeMap[typeof(uint)] = "INT";
            _typeMap[typeof(long)] = "INT";
            _typeMap[typeof(ulong)] = "INT";
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
            _typeMap[typeof(long?)] = "INT";
            _typeMap[typeof(ulong?)] = "INT";
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
    }
}