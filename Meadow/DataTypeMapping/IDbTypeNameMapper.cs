using System;
using System.Data;

namespace Meadow.DataTypeMapping
{
    public interface IDbTypeNameMapper
    {
        string this[Type type] { get; }
    }
}