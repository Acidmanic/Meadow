using System;
using System.Collections.Specialized;

namespace Meadow.Attributes
{
    public abstract class FieldAttribute : Attribute
    {
        public string Name { get; }

        public FieldAttribute(string name)
        {
            Name = name;
        }
    }
}