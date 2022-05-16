using System;
using System.Collections.Specialized;

namespace Meadow.Attributes
{
    public class ParameterAttribute : FieldAttribute
    {
        public ParameterAttribute(string name) : base(name)
        {
        }
    }
}