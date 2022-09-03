using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Extensions;

namespace Meadow.Test.Functional
{
    public class StandardIndexAccumulator<TModel>
    {
        private Dictionary<string,object> _addressedValues = new Dictionary<string,object>();
        private ObjectEvaluator _evaluator = new ObjectEvaluator(typeof(TModel));


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

            var belongingKey = Belongs(dpKey);

            if (!belongingKey)
            {
                Console.WriteLine($"The key {dpKey} does not belong to this data type, " +
                                  $"so it has been ignored.");
                return;
            }

            var foundKey = IsAnyInstancesPresent(dp.Identifier);
            
            if (foundKey)
            {

                Console.WriteLine($"New value {dp.Value} for {dp.Identifier}");
                
                var changed = IsChanged(foundKey, dp.Value);

                if (changed)
                {
                    var isUnique = IsUnique(foundKey);
                    
                    if (isUnique)
                    {
                        Console.WriteLine($"{dp.Identifier} points to a unique field, so it must be incremented.");
                        
                        var incremented = Increment(foundKey);

                        if (!incremented)
                        {
                            Console.WriteLine("The whole object got a new record");
                        }
                        else
                        {
                            Console.WriteLine($"key {foundKey.Value} has been incremented to {incremented.Value}");
                        
                            _addressedValues.Add(incremented.Value.ToString(),dp.Value);
                        
                        }    
                    }else
                    {
                        var oldValue = _addressedValues[foundKey.Value.ToString()];
                    
                        Console.WriteLine($"Received UnExpected value {dp.Value} " +
                                          $"on address {dp.Identifier} with old value {oldValue}");
                    }
                }
            }
            else
            {
                AddZeroedAllAddresses(dp);
                
                Console.WriteLine($"key {dp.Identifier} has been Added.");
            }
        }

        private void AddZeroedAllAddresses(DataPoint dp)
        {
            var zeroKey = FieldKey.Parse(dp.Identifier).ZeroIndexes();

            _addressedValues.Add(zeroKey.ToString(),dp.Value);
        }

        private Result<FieldKey> IsAnyInstancesPresent(string address)
        {
            var key = FieldKey.Parse(address);

            FieldKey found = null;
            
            foreach (var existingAddress in _addressedValues.Keys)
            {
                var exsKey = FieldKey.Parse(existingAddress);

                if (key.Equals(exsKey, FieldKeyComparisons.IgnoreAllIndexes))
                {
                    if (found == null || IsOnHigherIndex(exsKey, found))
                    {
                        found = exsKey;
                    }
                }
            }

            return new Result<FieldKey>(found != null, found);
        }

        private bool IsOnHigherIndex(FieldKey expectedHigher, FieldKey checkingAgainst)
        {
            for (int i = 0; i < expectedHigher.Count; i++)
            {
                if (expectedHigher[i].Indexed)
                {
                    if (expectedHigher[i].Index > checkingAgainst[i].Index)
                    {
                        return true;
                    }
                    if (expectedHigher[i].Index < checkingAgainst[i].Index)
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        private bool IsUnique(FieldKey key)
        {
            var evKey = Belongs(key).Value;

            var node = _evaluator.Map.NodeByKey(evKey);

            return node.IsAutoValued || node.IsUnique;
        }

        private Result<FieldKey> Belongs(FieldKey key)
        {
            var evKey = _evaluator.Map.Keys
                .FirstOrDefault(k => k.Equals(key, FieldKeyComparisons.IgnoreAllIndexes));

            return new Result<FieldKey>(evKey != null, evKey);
        }

        private bool IsChanged(FieldKey key, object incomingValue)
        {
            var address = key.ToString();
            
            if (!_addressedValues.ContainsKey(address))
            {
                throw new Exception("What the hell?");
            }

            var older = _addressedValues[address];

            if(older==null && incomingValue==null)
            {
                return false;
            }
            if(older==null || incomingValue==null)
            {
                return true;
            }
            return !Equals(older, incomingValue);
            
        }

       
        private Result<FieldKey> Increment(FieldKey k)
        {
            var key = FieldKey.Parse(k.ToString());
            
            for (int i = key.Count - 1; i >= 0; i--)
            {
                var segment = key[i];

                if (segment.Indexed)
                {
                    segment.Index++;

                    return new Result<FieldKey>(true, key);
                }
            }

            return new Result<FieldKey>(false, k);
        }

        public object DeliverShit()
        {
            var eva = new ObjectEvaluator(typeof(TModel));

            foreach (var dp in _addressedValues)
            {
                eva.Write(dp.Key,dp.Value);
            }

            return eva.RootObject;
        }
    }
}
