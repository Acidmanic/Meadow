using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.Bug2
{
    public class Card
    {
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
    }
}

