using Meadow.Attributes;

namespace Meadow.Test.Functional.Models
{
    public class Job
    {
        [UniqueField]
        public long Id { get; set; }

        public string Title { get; set; }

        public long IncomeInRials { get; set; }

        public string JobDescription { get; set; }
    }
}