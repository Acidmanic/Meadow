using System.Collections;
using System.Collections.Generic;

namespace Meadow.Reflection.ObjectTree.DataSource
{
    public class InMemoryDataStream : IDataStream
    {
        private readonly List<DataPoint> _internalData;

        public InMemoryDataStream(List<DataPoint> internalData)
        {
            _internalData = internalData;
        }


        public InMemoryDataStream()
        {
            _internalData = new List<DataPoint>();
        }

        public IEnumerator<DataPoint> GetEnumerator()
        {
            return _internalData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public InMemoryDataStream Add(string identifier, object value)
        {
            _internalData.Add(new DataPoint
            {
                Identifier = identifier,
                Value = value
            });
            return this;
        }
    }
}