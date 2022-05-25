using System;
using System.Collections.Generic;
using System.Data.Common;
using Meadow.Requests.FieldManipulation;

namespace Meadow.Reflection.ObjectTree.Mapping
{
    public class ObjectDataReader : ObjectDataMapperBase
    {
        public ObjectDataReader(object rootObject, bool fullTree) : base(new TypeAnalyzer().ToAccessNode(
            rootObject.GetType(), fullTree), rootObject)
        {
        }

        public Dictionary<string, object> ReadRootObject(IFieldMarks inputFieldMarks)
        {
            var result = new Dictionary<string, object>();

            foreach (var field in _treeInformation.OrderedFieldNames)
            {
                if (inputFieldMarks.IsIncluded(field))
                {
                    var leaf = _treeInformation[field];

                    var value = GetCorrespondingObject(leaf);

                    result.Add(field, value);
                }
            }

            return result;
        }
    }
}