using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Acidmanic.Utilities.Results;
using Meadow.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.Sql
{
    public class StandardIndexAccumulator<TModel>
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, object> _addressedValues;
        private readonly ObjectEvaluator _evaluator;
        private readonly IndexMap _indexMap;
        private readonly OuterKeyFirstDatapointComparer<TModel> _datapointComparer;
        private readonly HashSet<string> _appliedDataPoints;
        public List<Record> Records { get; }

        public StandardIndexAccumulator() : this(NullLogger.Instance)
        {
        }


        public StandardIndexAccumulator(ILogger logger)
        {
            _logger = logger;
            _addressedValues = new Dictionary<string, object>();
            _evaluator = new ObjectEvaluator(typeof(TModel));
            _indexMap = new IndexMap(typeof(TModel), DeliverCurrent);
            _datapointComparer = new OuterKeyFirstDatapointComparer<TModel>();
            _appliedDataPoints = new HashSet<string>();
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


        private string GetUniqueKey(DataPoint dataPoint)
        {
            var valueString = dataPoint.Value == null
                ? "null:null"
                : dataPoint.Value.GetType().FullName + ":" + dataPoint.Value.ToString();

            return dataPoint.Identifier + ":" + valueString;
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

                    var isApplied = IsApplied(dp);

                    if (changed)
                    {
                        _logger.LogTrace("{IncomingField} has been changed", incomingField);

                        if (isApplied)
                        {
                            _logger.LogTrace("{IncomingField} is already applied. Will Switch related " +
                                             "address without incrementation.", incomingField);
                            SwitchToValue(key, dp.Value);
                        }
                        else
                        {
                            var isUnique = IsUnique(incomingField);

                            _logger.LogTrace("An Increment will be applied on {Key}  ,...", key);

                            _indexMap.Increment(latestAddress);

                            latestAddress = _indexMap.GetLatest(latestAddress).ToString();

                            _logger.LogTrace("... and New Entity will be put under {LatestAddress}", latestAddress);

                            _addressedValues.Add(latestAddress, dp.Value);

                            if (!isUnique)
                            {
                                _logger.LogTrace("Value change on not-unique field {Key} has been detected. (bad sort)",
                                    key);
                            }
                        }
                    }
                }
                else
                {
                    _addressedValues.Add(latestAddress, dp.Value);
                }

                MarkApplied(dp);
            }
        }


        private List<Record> SortHomogeneously(IEnumerable<Record> records)
        {
            var idId = TypeIdentity.FindIdentityLeaf<TModel>().GetFullName();

            var sorted = new List<Record>();

            sorted.AddRange(records);

            var dirty = true;
            
            while (dirty)
            {
                dirty = false;

                var seen = new Dictionary<object, int>();

                object lastId = null;

                for (int i = 0; i < sorted.Count; i++)
                {
                    var idValue = sorted[i].FirstOrDefault(dp => dp.Identifier == idId)?.Value;

                    if (idValue != null)
                    {
                        if (!idValue.Equals(lastId)) // if change
                        {
                            if (seen.ContainsKey(idValue)) // if seen before
                            {
                                dirty = true;

                                MoveInto(sorted, i, seen[idValue] + 1);
                                
                                break;
                            }

                            lastId = idValue;

                            seen.Add(idValue, i);
                        }
                    }
                }
            }

            return sorted;
        }

        public static void MoveInto<T>(List<T> list, int index, int target)
        {
            var temp = list[index];

            for (int i = index-1; i >= target; i--)
            {
                list[i + 1] = list[i];
            }

            list[target] = temp;
        }
        
        public void PassAll(IEnumerable<Record> standardData)
        {
            Clear();

            _appliedDataPoints.Clear();

            var records = SortHomogeneously(standardData); 

            if (records.Count > 0)
            {
                foreach (var record in records)
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

        private bool IsApplied(DataPoint dp)
        {
            var key = GetUniqueKey(dp);

            return _appliedDataPoints.Contains(key);
        }

        private void MarkApplied(DataPoint dp)
        {
            var key = GetUniqueKey(dp);

            if (!_appliedDataPoints.Contains(key))
            {
                _appliedDataPoints.Add(key);
            }
        }

        private Result<FieldKey> Belongs(FieldKey key)
        {
            var evKey = _evaluator.Map.Keys
                .FirstOrDefault(k => k.Equals(key, FieldKeyComparisons.IgnoreAllIndexes));

            return new Result<FieldKey>(evKey != null, evKey);
        }

        private void SwitchToValue(FieldKey key, object incomingValue)
        {
            var address = key.ToString();

            if (!_addressedValues.ContainsKey(address))
            {
                throw new Exception("What the hell?");
            }

            _addressedValues.Remove(address);

            _addressedValues.Add(address, incomingValue);
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