using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

namespace Meadow.Sql
{
    public class SqlRecordAccumulator
    {
        private readonly Dictionary<AccessNode, int> _indexKeeper;
        private readonly Dictionary<string, object> _history;

        public SqlRecordAccumulator(AddressKeyNodeMap map)
        {
            Map = map;

            _indexKeeper = InitializeCollectionIndexes(map);
            
            _history = new Dictionary<string, object>();
        }

        private Dictionary<AccessNode, int> InitializeCollectionIndexes(AddressKeyNodeMap map)
        {
            var indexes = new Dictionary<AccessNode, int>();

            foreach (var node in map.Nodes)
            {
                if (node.IsCollectable)
                {
                    indexes.Add(node, 0);
                }
            }

            return indexes;
        }

        public AddressKeyNodeMap Map { get; }

        public List<Record> StandardRecords { get; } = new List<Record>();


        private int _currentRecordIndex = 0;

        private Record CurrentRecord
        {
            get
            {
                while (!(StandardRecords.Count > _currentRecordIndex))
                {
                    StandardRecords.Add(new Record());
                }

                return StandardRecords[_currentRecordIndex];
            }
        }

        public void Pass(object value, FieldProfile profile)
        {
            var foundCollectableParent = FindParentCollectableNode(profile);

            var actualAddress = ActualAddress(profile);

            if (foundCollectableParent)
            {
                if (SecondWriteOnSamePlace(value, profile))
                {
                    IncrementParentIndex(foundCollectableParent);

                    actualAddress = ActualAddress(profile);
                }
            }

            CurrentRecord.Add(actualAddress, value);

            LogIntoHistory(actualAddress, value);
        }


        private void LogIntoHistory(string actualAddress, object value)
        {
            if (_history.ContainsKey(actualAddress))
            {
                _history.Remove(actualAddress);
            }

            _history.Add(actualAddress, value);
        }

        private void IncrementParentIndex(FieldProfile parent)
        {
            if (_indexKeeper.ContainsKey(parent.Node))
            {
                _indexKeeper[parent.Node]++;
            }
        }

        private bool SecondWriteOnSamePlace(object value, FieldProfile profile)
        {
            if (profile.Node.IsUnique)
            {
                var actualAddress = ActualAddress(profile);

                if (_history.ContainsKey(actualAddress))
                {
                    var previousValue = _history[actualAddress];

                    if (previousValue != value)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private Result<FieldProfile> FindParentCollectableNode(FieldProfile profile)
        {
            var parent = profile;

            while (parent != null)
            {
                if (parent.Node.IsCollectable)
                {
                    return Result.Successful(parent);
                }

                parent = GetParent(parent);
            }

            return Result.Failure<FieldProfile>();
        }

        private FieldProfile GetParent(FieldProfile profile)
        {
            var parentNode = profile.Node.Parent;

            if (parentNode == null)
            {
                return null;
            }

            return new FieldProfile
            {
                Key = Map.FieldKeyByNode(parentNode),
                Node = parentNode
            };
        }


        private string ActualAddress(FieldProfile profile)
        {
            return ActualAddress(profile.Key);
        }

        private string ActualAddress(FieldKey key)
        {
            var subKey = new FieldKey();
            var actualKey = new FieldKey();

            for (int i = 0; i < key.Count ; i++)
            {
                var segment = key[i];

                subKey.Add(key[i]);

                if (segment.Indexed)
                {
                    int index = CurrentIndexFor(subKey);

                    segment = new Segment(segment.Name, index);
                }

                actualKey.Add(segment);
            }

            return actualKey.ToString();
        }

        private int CurrentIndexFor(FieldKey subKey)
        {
            var correspondingNode = Map.NodeByKey(subKey);

            if (_indexKeeper.ContainsKey(correspondingNode))
            {
                return _indexKeeper[correspondingNode];
            }

            return -1;
        }
    }
}