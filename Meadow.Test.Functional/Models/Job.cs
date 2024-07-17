using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models
{
    public class Job
    {
        [UniqueMember]
        [AutoValuedMember]
        public long Id { get; set; }

        public string Title { get; set; }

        public long IncomeInRials { get; set; }

        public string JobDescription { get; set; }
        
        public bool IsDeleted { get; set; } = false;
    }
}