using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Extensions;

namespace Meadow.Test.Functional
{
    public class IndexMap
    {
        private readonly Action OnNewObject;
        private readonly Dictionary<string, IIndexMapNode> _nodesByAddress;

        public IndexMap(Type type, Action onNewObject)
        {
            OnNewObject = onNewObject;
            _nodesByAddress = new Dictionary<string, IIndexMapNode>();

            var evaluator = new ObjectEvaluator(type);

            IndexNode(evaluator.RootNode, evaluator, _nodesByAddress, new NewObjectEventIndexMapNode(OnNewObject));
        }

        private IIndexMapNode IndexNode(AccessNode accessNode,
            ObjectEvaluator evaluator,
            Dictionary<string, IIndexMapNode> indexes, IIndexMapNode parent)
        {
            IIndexMapNode mapNode = accessNode.IsCollectable
                ? (IIndexMapNode) new CollectionIndexMapNode(parent)
                : (IIndexMapNode) new RedirectingIndexMapNode(parent);

            mapNode.ClearIndexAddress = evaluator.Map.AddressByNode(accessNode);

            var children = accessNode.GetChildren();

            foreach (var child in children)
            {
                var childMapNode = IndexNode(child, evaluator, indexes, mapNode);

                mapNode.Children.Add(childMapNode);
            }

            indexes.Add(mapNode.ClearIndexAddress, mapNode);

            return mapNode;
        }


        public FieldKey GetLatest(string address)
        {
            var clearField = FieldKey.Parse(address).ClearIndexes();

            var result = new FieldKey();

            var currentKey = new FieldKey();

            for (int i = 0; i < clearField.Count; i++)
            {
                currentKey.Add(clearField[i]);

                var currentAddress = currentKey.ToString();

                if (!_nodesByAddress.ContainsKey(currentAddress))
                {
                    Console.WriteLine($"NO SUCH ADDRESS POSSIBLE {currentAddress} of {address}");

                    return null;
                }

                var currentNode = _nodesByAddress[currentAddress];

                var currentIndex = currentNode.Index;

                var currentSegment = currentIndex == -1
                    ? new Segment(clearField[i].Name)
                    : new Segment(clearField[i].Name, currentIndex);

                result.Add(currentSegment);
            }

            return result;
        }

        public void Increment(string address)
        {
            var clearAddress = FieldKey.Parse(address).ClearIndexes().ToString();

            if (!_nodesByAddress.ContainsKey(clearAddress))
            {
                Console.WriteLine($"NO SUCH ADDRESS POSSIBLE {clearAddress} of {address}");

                return;
            }

            var node = _nodesByAddress[clearAddress];

            node.Increment();
        }
    }
}