using System;
using System.Collections.Generic;
using System.Data;
using Meadow.DataTypeMapping;

namespace Meadow.SqlServer
{
    public class SqlDbTypeNameMapper : SqlDbTypeNameMapperBase
    {
        private readonly Dictionary<Type, string> _typeMap = new Dictionary<Type, string>();

        public SqlDbTypeNameMapper()
        {
            _typeMap[typeof(byte)] = SqlDbType.TinyInt.ToString().ToLower();
            _typeMap[typeof(sbyte)] = SqlDbType.TinyInt.ToString().ToLower();
            _typeMap[typeof(short)] = SqlDbType.SmallInt.ToString().ToLower();
            _typeMap[typeof(ushort)] = SqlDbType.SmallInt.ToString().ToLower();
            _typeMap[typeof(int)] = SqlDbType.Int.ToString().ToLower();
            _typeMap[typeof(uint)] = SqlDbType.Int.ToString().ToLower();
            _typeMap[typeof(long)] = SqlDbType.BigInt.ToString().ToLower();
            _typeMap[typeof(ulong)] = SqlDbType.BigInt.ToString().ToLower();
            _typeMap[typeof(float)] = SqlDbType.Float.ToString().ToLower();
            _typeMap[typeof(double)] = SqlDbType.Real.ToString().ToLower();
            _typeMap[typeof(decimal)] = SqlDbType.Decimal.ToString().ToLower();
            _typeMap[typeof(bool)] = SqlDbType.Bit.ToString().ToLower();
            _typeMap[typeof(string)] = SqlDbType.NVarChar.ToString().ToLower() + "(256)";
            _typeMap[typeof(char)] = SqlDbType.NChar.ToString().ToLower() + "(1)";
            _typeMap[typeof(Guid)] = SqlDbType.UniqueIdentifier.ToString().ToLower();
            _typeMap[typeof(DateTime)] = SqlDbType.DateTime.ToString().ToLower();
            _typeMap[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset.ToString().ToLower();
            _typeMap[typeof(byte[])] = SqlDbType.Binary.ToString().ToLower();
            _typeMap[typeof(byte?)] = SqlDbType.Binary.ToString().ToLower();
            _typeMap[typeof(sbyte?)] = SqlDbType.Binary.ToString().ToLower();
            _typeMap[typeof(short?)] = SqlDbType.SmallInt.ToString().ToLower();
            _typeMap[typeof(ushort?)] = SqlDbType.SmallInt.ToString().ToLower();
            _typeMap[typeof(int?)] = SqlDbType.Int.ToString().ToLower();
            _typeMap[typeof(uint?)] = SqlDbType.Int.ToString().ToLower();
            _typeMap[typeof(long?)] = SqlDbType.BigInt.ToString().ToLower();
            _typeMap[typeof(ulong?)] = SqlDbType.BigInt.ToString().ToLower();
            _typeMap[typeof(float?)] = SqlDbType.Float.ToString().ToLower();
            _typeMap[typeof(double?)] = SqlDbType.Real.ToString().ToLower();
            _typeMap[typeof(decimal?)] = SqlDbType.Decimal.ToString().ToLower();
            _typeMap[typeof(bool?)] = SqlDbType.Bit.ToString().ToLower();
            _typeMap[typeof(char?)] = SqlDbType.NChar.ToString().ToLower() + "(1)";
            _typeMap[typeof(Guid?)] = SqlDbType.UniqueIdentifier.ToString().ToLower();
            _typeMap[typeof(DateTime?)] = SqlDbType.DateTime.ToString().ToLower();
            _typeMap[typeof(DateTimeOffset?)] = SqlDbType.DateTimeOffset.ToString().ToLower();
            //_typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;   
        }

        public override string GetDatabaseTypeName(Type type)
        {
            return _typeMap[type];
        }

        protected override string GetLargeTextDataType()
        {
            return "NVARCHAR(MAX)";
        }
    }
}