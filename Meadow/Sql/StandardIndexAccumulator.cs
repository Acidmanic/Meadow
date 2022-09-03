using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Extensions;

namespace Meadow.Sql
{
    public class StandardIndexAccumulator<TModel>
    {
        private readonly Dictionary<string, object> _addressedValues;
        private readonly ObjectEvaluator _evaluator;
        private readonly IndexMap _indexMap;
        private readonly OuterKeyFirstDatapointComparer<TModel> _datapointComparer;

        public List<Record> Records { get; }

        public StandardIndexAccumulator()
        {
            _addressedValues = new Dictionary<string, object>();
            _evaluator = new ObjectEvaluator(typeof(TModel));
            _indexMap = new IndexMap(typeof(TModel), DeliverCurrent);
            _datapointComparer = new OuterKeyFirstDatapointComparer<TModel>();
            Records = new List<Record>();
        }

        public void Clear()
        {
            _addressedValues.Clear();
            _indexMap.Clear();
        }

        public void EndOfRecordStream()
        {
            DeliverCurrent();
        }


        private void Pass(DataPoint dp)
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
                        Console.WriteLine($"An Increment will be applied on {key}  ,...");

                        _indexMap.Increment(latestAddress);

                        latestAddress = _indexMap.GetLatest(latestAddress).ToString();

                        Console.WriteLine($"... and New Entity will be put under {latestAddress}");

                        _addressedValues.Add(latestAddress, dp.Value);

                        if (!isUnique)
                        {
                            Console.WriteLine($"Value change on not-unique field {key} has been detected. (bad sort)");
                        }
                    }
                }
                else
                {
                    _addressedValues.Add(latestAddress, dp.Value);
                }
            }
        }


        public void PassAll(IEnumerable<Record> standardData)
        {
            Clear();

            if (standardData.Any())
            {
                foreach (var record in standardData)
                {
                    record.Sort(_datapointComparer);

                    foreach (var dp in record)
                    {
                        Pass(dp);
                    }
                }

                EndOfRecordStream();
            }
        }

        private void DeliverCurrent()
        {
            var record = new Record();

            foreach (var dp in _addressedValues)
            {
                record.Add(dp.Key, dp.Value);
            }

            Records.Add(record);

            Clear();
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
    }
}