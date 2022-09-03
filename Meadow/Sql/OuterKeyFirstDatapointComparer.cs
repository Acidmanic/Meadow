using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

namespace Meadow.Sql
{
    public class OuterKeyFirstDatapointComparer : Comparer<DataPoint>
    {
        private readonly ObjectEvaluator _evaluator;

        public OuterKeyFirstDatapointComparer(Type modelType)
        {
            _evaluator = new ObjectEvaluator(modelType);
        }

        public override int Compare(DataPoint x, DataPoint y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentException("Data points can not be null.");
            }

            var deltaExistence = Compare(
                _evaluator.Map.Addresses.Contains(x.Identifier),
                _evaluator.Map.Addresses.Contains(y.Identifier));

            if (deltaExistence != 0)
            {
                return deltaExistence;
            }

            var xNode = _evaluator.Map.NodeByAddress(x.Identifier);
            var yNode = _evaluator.Map.NodeByAddress(y.Identifier);

            var deltaDepth = xNode.Depth - yNode.Depth;

            if (deltaDepth != 0)
            {
                return deltaDepth;
            }

            var deltaUnique = Compare(xNode.IsUnique, yNode.IsUnique);

            if (deltaUnique != 0)
            {
                return deltaUnique;
            }

            var deltaAutoValued = Compare(xNode.IsAutoValued, yNode.IsAutoValued);

            return deltaAutoValued;
        }

        // FOr Ascending
        private int Compare(bool xGood, bool yGood)
        {
            if (xGood && yGood)
            {
                return 0;
            }

            if (xGood)
            {
                return -1;
            }

            if (yGood)
            {
                return 1;
            }

            return 0;
        }
    }

    public class OuterKeyFirstDatapointComparer<TModel> : OuterKeyFirstDatapointComparer
    {
        public OuterKeyFirstDatapointComparer() : base(typeof(TModel))
        {
        }
    }
}