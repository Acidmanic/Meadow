using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Utility;

namespace Meadow.Sql
{
    public class StandardDataPointComparator:ComparatorBase<DataPoint>
    {

        private readonly Dictionary<string, FieldProfile> _mappedFieldIds;

        public StandardDataPointComparator(Dictionary<string, FieldProfile> mappedFieldIds)
        {
            this._mappedFieldIds = mappedFieldIds;
        }

        protected override int CompareNotNull(DataPoint x, DataPoint y)
        {

            var bothExistsCompare = SubCompare(o => _mappedFieldIds.ContainsKey(o.Identifier), x, y);

            if (bothExistsCompare.WereEqual)
            {
                if (!bothExistsCompare.XApplies)
                {
                    // neither exist
                    return 0;
                }
                // both exist
                var xNode = _mappedFieldIds[x.Identifier].Node;
                var yNode = _mappedFieldIds[y.Identifier].Node;
            
                return new AccessNodeComparator().Compare(xNode,yNode);    
            }
            return bothExistsCompare.Comparison;
            
        }


    }
}