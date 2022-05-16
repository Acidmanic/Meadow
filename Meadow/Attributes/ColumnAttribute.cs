using System;
using System.Collections.Specialized;

namespace Meadow.Attributes
{
    public class ColumnAttribute : FieldAttribute
    {
        public ColumnAttribute(string name) : base(name)
        {
        }
    }
}