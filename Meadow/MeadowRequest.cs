using System;
using System.Collections.Generic;

namespace Meadow
{
    public class MeadowRequest<TIn, TOut>
        where TOut : class, new()
    {
        public TIn ToStorage { get; set; }

        public List<TOut> FromStorage { get; set; }

        public string RequestName { get; set; }

        public bool ReturnsValue { get; }

        public MeadowRequest(bool returnsValue)
        {
            ReturnsValue = returnsValue;
        }
    }
}