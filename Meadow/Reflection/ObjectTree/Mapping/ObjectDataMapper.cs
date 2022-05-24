using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using Meadow.Reflection.ObjectTree.DataSource;

namespace Meadow.Reflection.ObjectTree.Mapping
{
    public class ObjectDataMapper
    {
        private readonly AccessNode _rootNode;

        private readonly object _rootObject;

        private readonly Dictionary<string, AccessNode> _leavesById;

        private readonly List<AccessNode> _orderedLeaves;

        private readonly Dictionary<AccessNode, object> _writeHistory;

        private readonly Dictionary<string, int> _fieldsOrders;

        public ObjectDataMapper(AccessNode rootNode)
        {
            _rootNode = rootNode;

            _rootObject = new TypeAnalyzer().CreateObject(_rootNode.Type);

            _orderedLeaves = rootNode.EnumerateLeavesBelow();

            _orderedLeaves.Sort(new AccessNodeComparator());


            _fieldsOrders = new Dictionary<string, int>();
            _leavesById = new Dictionary<string, AccessNode>();
            _writeHistory = new Dictionary<AccessNode, object>();

            EnumerateLeaves(_orderedLeaves);
        }


        private void EnumerateLeaves(List<AccessNode> leaves)
        {
            _leavesById.Clear();
            _fieldsOrders.Clear();

            var counts = CountFieldNames(leaves);

            for (int leafIndex = 0; leafIndex < leaves.Count; leafIndex++)
            {
                var leaf = leaves[leafIndex];

                var name = leaf.Name;

                if (counts[name] > 1)
                {
                    name = leaf.Parent.Name + "." + name;
                }

                _leavesById.Add(name, leaf);
                _fieldsOrders.Add(name, leafIndex);
            }
        }

        public Dictionary<string, int> CountFieldNames(List<AccessNode> nodes)
        {
            Dictionary<string, int> fieldCount = new Dictionary<string, int>();

            foreach (var node in nodes)
            {
                var field = node.Name;

                if (fieldCount.ContainsKey(field))
                {
                    fieldCount[field] += 1;
                }
                else
                {
                    fieldCount.Add(field, 1);
                }
            }

            return fieldCount;
        }

        public void Write(IDataReader dataReader)
        {
            var drFields = EnumFields(dataReader);


            while (dataReader.Read())
            {
                var record = new List<DataPoint>();

                foreach (var field in drFields)
                {
                    var value = dataReader[field];

                    if (!(value is DBNull) && value != null)
                    {
                        var datapoint = new DataPoint
                        {
                            Identifier = field,
                            Value = dataReader[field]
                        };
                        record.Add(datapoint);
                    }
                }

                record.Sort(new DataPointComparator(_fieldsOrders));

                record.ForEach(WriteData);
            }
        }

        private List<string> EnumFields(IDataReader dataReader)
        {
            var result = new List<string>();

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                result.Add(dataReader.GetName(i));
            }

            return result;
        }

        private void WriteData(DataPoint point)
        {
            var fieldName = point.Identifier;

            if (_leavesById.ContainsKey(fieldName))
            {
                var leaf = _leavesById[fieldName];

                var data = point.Value;

                var topLevelNode = leaf.GetTopLevelNode();

                if (topLevelNode == null)
                {
                    // Impossible! Leaves can not be root-collections!!
                    throw new Exception("Impossible! Leaves can not be root-collections!!");
                }

                if (topLevelNode.IsRoot)
                {
                    PerformLeafSet(leaf, _rootObject, data);
                }
                else
                {
                    var set = false;

                    while (!set)
                    {
                        var topLevelObject = GetCorrespondingObject(topLevelNode);

                        set = PerformLeafSet(leaf, topLevelObject, data);
                    }
                }
            }
            else
            {
                // Field Not Found ( received data is not defined in the model)
                Console.WriteLine($"No Field Found for: {point.Identifier}: {point.Value}");
            }
        }

        private bool AreEqual(object o1, object o2)
        {
            if (o1 == null && o2 == null)
            {
                return true;
            }

            if (o1 == null || o2 == null)
            {
                return false;
            }

            return o1.Equals(o2);
        }

        private bool PerformLeafSet(AccessNode leaf, object toplevelObject, object value)
        {
            if (_writeHistory.ContainsKey(leaf))
            {
                var oldValue = _writeHistory[leaf];

                if (!AreEqual(oldValue, value))
                {
                    if (leaf.IsUnique)
                    {
                        if (leaf.Parent.IsCollectable)
                        {
                            Console.WriteLine(
                                $"New Record Detected for {leaf.Parent.Name}.{leaf.Name}, {oldValue} -> {value}");

                            AddCommanded(leaf, toplevelObject);
                        }
                        else
                        {
                            Console.WriteLine("This is wiered");
                        }

                        return false;
                    }
                }
            }
            else
            {
                leaf.SetValue(toplevelObject, value);
                _writeHistory.Add(leaf, value);
            }

            return true;
        }

        private void AddCommanded(AccessNode leaf, object toplevelObject)
        {
            var elementType = leaf.Parent.Type;

            var element = new TypeAnalyzer().CreateObject(elementType);

            var collectionNode = leaf.Parent.Parent;

            var collectionObject = GetCorrespondingObject(collectionNode);

            new CollectionCollection(collectionObject as ICollection).Add(element);

            ClearHistoryFor(leaf.Parent);
        }

        private void ClearHistoryFor(AccessNode node)
        {
            var leaves = node.EnumerateLeavesBelow();

            leaves.ForEach(l => _writeHistory.Remove(l));
        }

        private object GetCorrespondingObject(AccessNode node)
        {
            var topLevelNode = node.GetTopLevelNode();

            if (topLevelNode == null)
            {
                //Root Collection or collectable after it
                if (node.IsRoot)
                {
                    return _rootObject;
                }
                else if (node.IsCollectable && node.Parent.IsRoot)
                {
                    return GetLastObject(_rootObject);
                }
                else
                {
                    throw new Exception("Impossible!");
                }
            }

            if (topLevelNode.IsRoot)
            {
                //Normal
                return node.GetValue(_rootObject);
            }

            // Collectable
            if (node.IsCollectable)
            {
                var collectionNode = node.Parent;

                var collectionObject = GetCorrespondingObject(collectionNode);

                var toplevelObject = GetLastObject(collectionObject);

                return node.GetValue(toplevelObject);
            }
            //  normal

            var topLevelObject = GetCorrespondingObject(topLevelNode);

            return node.GetValue(topLevelObject);
        }


        private object GetLastObject(object collectionObject)
        {
            if (collectionObject is ICollection collection)
            {
                var wraped = new CollectionCollection(collection);

                if (wraped.Count == 0)
                {
                    var newElement = new TypeAnalyzer().CreateObject(wraped.ElementType);

                    wraped.Add(newElement);
                }

                return wraped.Last();
            }

            throw new Exception(" It supposed to be a collection but it was not");
        }

        public object RootObject => _rootObject;
    }
}