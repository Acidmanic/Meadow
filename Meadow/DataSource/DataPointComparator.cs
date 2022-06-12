using System.Collections.Generic;

namespace Meadow.DataSource
{
    public class DataPointComparator : Comparer<DataPoint>
    {
        private readonly Dictionary<string, int> _leavesByOrder;

        public DataPointComparator(Dictionary<string, int> leavesByOrder)
        {
            this._leavesByOrder = leavesByOrder;
        }


        public override int Compare(DataPoint x, DataPoint y)
        {
            var order1 = GetOrder(x);
            var order2 = GetOrder(y);

            return order1 - order2;
        }

        private int GetOrder(DataPoint dataPoint)
        {
            if (_leavesByOrder.ContainsKey(dataPoint.Identifier))
            {
                return _leavesByOrder[dataPoint.Identifier];
            }

            return int.MaxValue;
        }
    }
}