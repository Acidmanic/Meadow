using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

namespace Meadow.Sql
{
    public class StandardDataPointComparator:IComparer<DataPoint>
    {

        private readonly Dictionary<string, FieldProfile> _mappedFieldIds;

        public StandardDataPointComparator(Dictionary<string, FieldProfile> mappedFieldIds)
        {
            this._mappedFieldIds = mappedFieldIds;
        }

        public int Compare(DataPoint x, DataPoint y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }
            var xNode = _mappedFieldIds[x.Identifier].Node;
            var yNode = _mappedFieldIds[y.Identifier].Node;
            
            return new AccessNodeComparator().Compare(xNode,yNode);
        }
    }
}