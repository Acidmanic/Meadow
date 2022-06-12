using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    public class SupplementDal
    {
        [UniqueMember]
        public long Id { get; set; }
        
        public ProductClassDal UniqueProductClass { get; set; }
        
        public long UniqueProductClassId { get; set; }
        
        public double Stock { get; set; }
        
        public int ValidDate { get; set; }
        
        public int MinVolume { get; set; }
        
        public int MaxVolume { get; set; }
        
        public bool AutoBid { get; set; }
    }
}