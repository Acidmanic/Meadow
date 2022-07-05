using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

namespace Meadow.SqlServer
{
    public class KeyDataPoint
    {
        public FieldKey Key { get; set; }

        public object Value { get; set; }

        public KeyDataPoint(FieldKey key, object value)
        {
            Key = key;
            Value = value;
        }

        public KeyDataPoint(string standardAddress, object value)
        {
            Key = FieldKey.Parse(standardAddress);

            value = value;
        }

        public KeyDataPoint(DataPoint dataPoint) : this(dataPoint.Identifier, dataPoint.Value)
        {
        }

        public DataPoint ToDataPoint()
        {
            return new DataPoint
            {
                Identifier = Key.ToString(),
                Value = Value
            };
        }
    }
}