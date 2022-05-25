using System;
using System.Collections.Generic;
using System.Diagnostics;
using Meadow.Reflection.ObjectTree;

namespace Meadow.Reflection
{
    public class AccessTreeInformation
    {
        private readonly Dictionary<string, AccessNode> _leavesById;

        private readonly List<AccessNode> _orderedLeaves;

        private readonly Dictionary<string, int> _fieldsOrders;

        private string[] _orderedFieldNames;
        
        public AccessTreeInformation(AccessNode tree)
        {
            _orderedLeaves = tree.EnumerateLeavesBelow();

            _orderedLeaves.Sort(new AccessNodeComparator());

            _orderedFieldNames = new string[0];
            _fieldsOrders = new Dictionary<string, int>();
            _leavesById = new Dictionary<string, AccessNode>();

            EnumerateLeaves(_orderedLeaves);
        }
        
        private void EnumerateLeaves(List<AccessNode> leaves)
        {
            _leavesById.Clear();
            _fieldsOrders.Clear();

            var counts = CountFieldNames(leaves);

            _orderedFieldNames = new String[leaves.Count];

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
                _orderedFieldNames[leafIndex] = name;
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


        public AccessNode this[string fieldName] => _leavesById[fieldName];

        public AccessNode this[int fieldOrder] => _orderedLeaves[fieldOrder];

        public int FieldsCount => _orderedLeaves.Count;

        public int GetOrderOf(string fieldName)
        {
            return _fieldsOrders[fieldName];
        }
        
        public Dictionary<string,int> GetFieldsOrders() => new Dictionary<string, int>(_fieldsOrders);

        public bool HasField(string fieldName) => _leavesById.ContainsKey(fieldName);

        public List<AccessNode> OrderedLeaves => _orderedLeaves;

        public string[] OrderedFieldNames => _orderedFieldNames;
    }
}