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
        private readonly Dictionary<string, object> _addressedValues = new Dictionary<string, object>();
        private readonly ObjectEvaluator _evaluator = new ObjectEvaluator(typeof(TModel));
        private IndexMap _indexMap = new IndexMap(typeof(TModel), () => Console.WriteLine("Dude make new Object!"));

        public StandardIndexAccumulator()
        {
        }

        public void Clear()
        {
        }

        public void Pass(DataPoint dp)
        {
            var incomingField = FieldKey.Parse(dp.Identifier).ClearIndexes();

            var incomingAddress = incomingField.ToString();

            var key = _indexMap.GetLatest(incomingAddress);

            if (key != null)
            {
                var latestAddress = key.ToString();

                if (_addressedValues.ContainsKey(latestAddress))
                {
                    var changed = IsChanged(key, dp.Value);

                    if (changed)
                    {
                        Console.WriteLine($"{incomingField} has been changed");

                        var isUnique = IsUnique(incomingField);

                        if (isUnique)
                        {
                            Console.WriteLine($"An Increment will be applied on {key}  ,...");

                            _indexMap.Increment(latestAddress);

                            latestAddress = _indexMap.GetLatest(latestAddress).ToString();

                            Console.WriteLine($"... and New Entity will be put under {latestAddress}");

                            _addressedValues.Add(latestAddress, dp.Value);
                        }
                        else
                        {
                            Console.WriteLine($"Value change on not-unique field {key}, has been ignored. (bad sort)");
                        }
                    }
                }
                else
                {
                    _addressedValues.Add(latestAddress, dp.Value);
                }
            }
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

            if (older == null && incomingValue == null)
            {
                return false;
            }

            if (older == null || incomingValue == null)
            {
                return true;
            }

            return !Equals(older, incomingValue);
        }


        public object DeliverShit()
        {
            var eva = new ObjectEvaluator(typeof(TModel));

            foreach (var dp in _addressedValues)
            {
                eva.Write(dp.Key, dp.Value);
            }

            return eva.RootObject;
        }
    }
}