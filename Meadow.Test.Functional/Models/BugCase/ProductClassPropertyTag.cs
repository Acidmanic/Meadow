namespace Meadow.Test.Functional.Models.BugCase
{
    public class ProductClassPropertyTag
    {
        public ProductClassPropertyTag()
        {
        }
        
        public long PropertyId { get; set; }

        public PropertyDal Property { get; set; }

        public long ProductClassId { get; set; }

        public ProductClassDal ProductClass { get; set; }
    }
}