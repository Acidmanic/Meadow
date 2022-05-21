using System;

namespace Meadow.Attributes
{
    public class TableAttribute:Attribute
    {
        public string TableName { get; }

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}