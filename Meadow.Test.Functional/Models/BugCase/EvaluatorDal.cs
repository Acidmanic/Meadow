using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    [OwnerName("Evaluators")]
    public class EvaluatorDal
    {
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        
        public int Type { get; set; }
        
        public double Minimum { get; set; }
        
        public double Maximum { get; set; }
        
        public double Step { get; set; }
        
        public string Values { get; set; }
        
        [UniqueMember]
        public string DefinitionUniqueId { get; set; }
        
    }
}