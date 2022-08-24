using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    [OwnerName("Quantifiers")]
    public class QuantifierDal
    {
        public EvaluatorDal Evaluator { get; set; }

        public string Unit { get; set; }


        [AutoValuedMember] [UniqueMember] public long Id { get; set; }

        public long EvaluatorId { get; set; }
        
        [UniqueMember]
        public string DefinitionUniqueId { get; set; }
        
    }
}