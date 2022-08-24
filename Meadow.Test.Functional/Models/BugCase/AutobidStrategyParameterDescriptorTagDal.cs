using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    
    [OwnerName("AutobidStrategyParameterDescriptorTags")]
    public class AutobidStrategyParameterDescriptorTagDal
    {
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        
        public long AutobidStrategyId { get; set; }
        
        public long StrategyParameterDescriptorId { get; set; }
        
    }
}