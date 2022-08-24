using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    [OwnerName("StrategyParameters")]
    public class StrategyParameterDal
    {
        [AutoValuedMember] [UniqueMember] public long Id { get; set; }

        public double Value { get; set; }

        public string Name { get; set; }

        public long SupplementId { get; set; }

        [UniqueMember] public long StrategyParameterDescriptorId { get; set; }

        public StrategyParameterDescriptorDal StrategyParameterDescriptor { get; set; }
    }
}