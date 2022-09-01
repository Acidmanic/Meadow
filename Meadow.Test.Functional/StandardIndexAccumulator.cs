using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

namespace Meadow.Test.Functional
{
    public class StandardIndexAccumulator
    {
        private List<FieldKey> _addresses = new List<FieldKey>();
        // private Dictionary<FieldKey, object> _dataByKeies;


        public StandardIndexAccumulator()
        {
            // _dataByKeies = new Dictionary<FieldKey, object>();
        }


        public void Clear()
        {
            // _dataByKeies.Clear();
        }


        public void Pass(DataPoint dp)
        {
            // if key exists with same index  inc index
            // if not exists, add key

            var dpKey = FieldKey.Parse(dp.Identifier);

            var key = _addresses
                .FirstOrDefault(k => k.Equals(dpKey, FieldKeyComparisons.IgnoreAllIndexes));

            if (key != null)
            {
                Console.WriteLine("Must increment : " + key);
            }
            else
            {
                _addresses.Add(dpKey);
            }
        }
    }
}