using System;

namespace Meadow.Reflection.Conventions
{
    public interface ITableNameProvider
    {
        string GetTableName(Type type);
    }
}