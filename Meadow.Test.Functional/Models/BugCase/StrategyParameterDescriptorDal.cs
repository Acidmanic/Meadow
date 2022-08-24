using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    [OwnerName("StrategyParameterDescriptors")]
    public class StrategyParameterDescriptorDal
    {
        
        [AutoValuedMember][UniqueMember]
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        [UniqueMember]
        public string DefinitionUniqueId { get; set; }
        
        public long QuantifierId { get; set; }
        
        public QuantifierDal Quantifier { get; set; }
        
    }
}