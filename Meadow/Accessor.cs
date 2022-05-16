using System;

namespace Meadow
{
    public class Accessor
    {
        /// <summary>
        /// Returns the property value from the given object
        /// </summary>
        public Func<object,object> Getter { get; set; }
        /// <summary>
        /// Sets given value as a property into given object
        /// </summary>
        public Action<object,object> Setter { get; set; }
    }
}