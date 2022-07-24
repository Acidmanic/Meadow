using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.SqlServer.Models
{
    public class Job
    {
        [UniqueMember]
        public long Id { get; set; }

        public string Title { get; set; }

        public long IncomeInRials { get; set; }

        public string JobDescription { get; set; }
    }
}