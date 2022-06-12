using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.Sets;

namespace Meadow.Reflection.Mapping
{
    public abstract class ObjectDataMapperBase
    {
        protected readonly object _rootObject;

        protected readonly AccessTreeInformation _treeInformation;

        public ObjectDataMapperBase(AccessNode rootNode,object rootObject)
        {
            _rootObject = rootObject;

            _treeInformation = new AccessTreeInformation(rootNode);

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

        protected bool AreEqual(object o1, object o2)
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

        protected object GetCorrespondingObject(AccessNode node)
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
                var wrapped = new CollectionCollection(collection);

                if (wrapped.Count == 0)
                {
                    var newElement = new TypeAnalyzer().CreateObject(wrapped.ElementType);

                    wrapped.Add(newElement);
                }

                return wrapped.Last();
            }

            throw new Exception(" It supposed to be a collection but it was not");
        }

        public object RootObject => _rootObject;
    }
}