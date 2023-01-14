using System.Collections.Generic;

namespace Meadow.Test.Functional.Models.Bug2
{
    public class Step:Card
    {
        public long ProjectId { get; set; }

        public long GoalId { get; set; }
        
        public List<Task> Tasks { get; set; }


    }
}