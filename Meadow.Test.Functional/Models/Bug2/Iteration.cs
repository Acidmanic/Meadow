using System.Collections.Generic;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.Bug2
{
    [OwnerName("Iterations")]
    public class Iteration
    {
        [AutoValuedMember] [UniqueMember] public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        
        public long ProjectId { get; set; }
        
        public List<Task> Tasks { get; set; }
    }
}