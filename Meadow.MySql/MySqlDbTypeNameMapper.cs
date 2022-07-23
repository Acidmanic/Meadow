using System;
using System.Collections.Generic;
using System.Data;
using Meadow.DataTypeMapping;
using MySql.Data.MySqlClient;

namespace Meadow.MySql
{
    public class MySqlDbTypeNameMapper : IDbTypeNameMapper
    {
        private readonly Dictionary<Type, string> _typeMap = new Dictionary<Type, string>();

        public string this[Type type]
        {
            get
            {
                if (type.IsEnum)
                {
                    return SqlDbType.Int.ToString().ToLower();
                }

                return _typeMap[type];
            }
        }

        public MySqlDbTypeNameMapper()
        {
            _typeMap[typeof(byte)] = "TINYINT(size)";
            _typeMap[typeof(sbyte)] = "TINYINT(size)";
            _typeMap[typeof(short)] = "SMALLINT(7)";
            _typeMap[typeof(ushort)] = "SMALLINT(7)";
            _typeMap[typeof(int)] = "INT(10)";
            _typeMap[typeof(uint)] = "INT(10)";
            _typeMap[typeof(long)] = "BIGINT(16)";
            _typeMap[typeof(ulong)] = "BIGINT(16)";
            _typeMap[typeof(float)] = MySqlDbType.Float.ToString().ToLower();
            _typeMap[typeof(double)] = MySqlDbType.Double.ToString().ToLower();
            _typeMap[typeof(decimal)] = MySqlDbType.Decimal.ToString().ToLower();
            _typeMap[typeof(bool)] = "BOOL";
            _typeMap[typeof(string)] = MySqlDbType.Text.ToString().ToLower() + "(256)";
            _typeMap[typeof(char)] = MySqlDbType.VarChar.ToString().ToLower() + "(1)";
            _typeMap[typeof(Guid)] = MySqlDbType.VarChar.ToString().ToLower() + "(40)";
            _typeMap[typeof(DateTime)] = MySqlDbType.DateTime.ToString().ToLower();
            _typeMap[typeof(DateTimeOffset)] = MySqlDbType.DateTime.ToString().ToLower();
            _typeMap[typeof(byte[])] = MySqlDbType.Binary.ToString().ToLower();
            _typeMap[typeof(byte?)] = "TINYINT(size)";
            _typeMap[typeof(sbyte?)] = "TINYINT(size)";
            _typeMap[typeof(short?)] = "SMALLINT(7)";
            _typeMap[typeof(ushort?)] = "SMALLINT(7)";
            _typeMap[typeof(int?)] = "INT(10)";
            _typeMap[typeof(uint?)] = "INT(10)";
            _typeMap[typeof(long?)] = "BIGINT(16)";
            _typeMap[typeof(ulong?)] = "BIGINT(16)";
            _typeMap[typeof(float?)] = MySqlDbType.Float.ToString().ToLower();
            _typeMap[typeof(double?)] = MySqlDbType.Double.ToString().ToLower();
            _typeMap[typeof(decimal?)] = MySqlDbType.Decimal.ToString().ToLower();
            _typeMap[typeof(bool?)] = "BOOL";
            _typeMap[typeof(char?)] = MySqlDbType.VarChar.ToString().ToLower() + "(1)";
            _typeMap[typeof(Guid?)] = MySqlDbType.VarChar.ToString().ToLower() + "(40)";
            _typeMap[typeof(DateTime?)] = MySqlDbType.DateTime.ToString().ToLower();
            _typeMap[typeof(DateTimeOffset?)] = MySqlDbType.DateTime.ToString().ToLower();
            //_typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;   
        }
    }
}