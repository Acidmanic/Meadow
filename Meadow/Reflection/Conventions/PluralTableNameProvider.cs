using System;
using System.Linq;
using System.Reflection;
using Meadow.Attributes;

namespace Meadow.Reflection.Conventions
{
    public class PluralTableNameProvider : ITableNameProvider
    {
        public string GetTableName(Type type)
        {
            var attributes = type.GetCustomAttributes<TableAttribute>().ToList();

            if (attributes.Count > 0)
            {
                return attributes.Last().TableName;
            }

            var name = type.Name;

            if (name.EndsWith("s") || name.EndsWith("S"))
            {
                name += "e";
            }

            return name + "s";
        }
    }
}