using System;
using System.Collections.Generic;

namespace Meadow
{
    public class MeadowRequest<TIn, TOut>
        where TOut : class, new()
    {
        public TIn ToStorage { get; set; }

        public List<TOut> FromStorage { get; set; }

        public string RequestText { get; protected set; }

        public bool ReturnsValue { get; }

        public MeadowRequest(bool returnsValue)
        {
            ReturnsValue = returnsValue;
            // ReSharper disable once VirtualMemberCallInConstructor
            RequestText = GetRequestText();
        }

        protected virtual string GetRequestText()
        {
            var name = this.GetType().Name;

            if (name.ToLower().EndsWith("request"))
            {
                name = name.Substring(0, name.Length - "request".Length);
            }
            return "sp" + name;
        }
    }
}