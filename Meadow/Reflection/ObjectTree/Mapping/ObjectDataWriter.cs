using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Meadow.Reflection.ObjectTree.DataSource;
using Meadow.Reflection.Sets;

namespace Meadow.Reflection.ObjectTree.Mapping
{
    public class ObjectDataWriter : ObjectDataMapperBase
    {
        private readonly Dictionary<AccessNode, object> _writeHistory;

        public ObjectDataWriter(AccessNode node) : base(node, new TypeAnalyzer().CreateObject(node.Type))
        {
            _writeHistory = new Dictionary<AccessNode, object>();
        }

        public ObjectDataWriter(Type type, bool fullTree) : base(new TypeAnalyzer().ToAccessNode(type, fullTree),
            new TypeAnalyzer().CreateObject(type))
        {
            _writeHistory = new Dictionary<AccessNode, object>();
        }

        public static ObjectDataWriter Create<TModel>(bool fullTree)
        {
            return new ObjectDataWriter(typeof(TModel), fullTree);
        }

        public void WriteIntoRootObject(IDataReader dataReader)
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

                record.Sort(new DataPointComparator(_treeInformation.GetFieldsOrders()));

                record.ForEach(WriteData);
            }
        }

        private void WriteData(DataPoint point)
        {
            var fieldName = point.Identifier;

            if (_treeInformation.HasField(fieldName))
            {
                var leaf = _treeInformation[fieldName];

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
                            AddNewElementToTopLevelCollection(leaf);
                        }
                        else
                        {
                            Console.WriteLine("This is weired");
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

        private void AddNewElementToTopLevelCollection(AccessNode leaf)
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

        private List<string> EnumFields(IDataReader dataReader)
        {
            var result = new List<string>();

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                result.Add(dataReader.GetName(i));
            }

            return result;
        }

        public T As<T>()
        {
            return (T) _rootObject;
        }
    }
}