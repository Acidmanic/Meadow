using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    public class ProductClassPropertyTag
    {
        public ProductClassPropertyTag()
        {
        }
        
        [MemberName("PropertyId")]
        public long PropertyId { get; set; }
        
        [MemberName("ProductClassId")]
        public long ProductClassId { get; set; }

        public PropertyDal Property { get; set; }

        [UniqueMember]
        public string Id
        {
            get
            {
                return $"{ProductClassId}:{PropertyId}";
            }
            set
            {
                // just ignore it!
            }
        }
    }
}