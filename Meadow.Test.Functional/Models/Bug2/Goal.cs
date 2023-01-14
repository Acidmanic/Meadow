using System.Collections.Generic;

namespace Meadow.Test.Functional.Models.Bug2
{
    public class Goal:Card
    {
        public long ProjectId { get; set; }
        
        public List<Step> Steps { get; set; }
    }
}