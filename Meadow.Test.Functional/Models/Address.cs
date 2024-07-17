using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models
{
    public class Address
    {
        public string City { get; set; }
        
        public string Street { get; set; }
        
        public string AddressName { get; set; }
        
        public int Block { get; set; }
        
        public int Plate { get; set; }
        
        [UniqueMember]
        [AutoValuedMember]
        public long Id { get; set; }
        
        public long PersonId { get; set; }
        
        public bool IsDeleted { get; set; } = false;
    }
}