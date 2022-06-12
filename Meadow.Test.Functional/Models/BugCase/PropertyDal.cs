using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    public class PropertyDal
    {
        public string StringValue { get; set; }

        public PropertyTypeDal Type { get; set; }

        public long TypeId { get; set; }

        [UniqueMember] public long Id { get; set; }
    }
}