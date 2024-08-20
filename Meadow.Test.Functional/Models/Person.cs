using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Attributes;

namespace Meadow.Test.Functional.Models
{
    public class Person
    {
        [UniqueMember]
        [AutoValuedMember]
        public long Id { get; set; }

        [CollectiveIdentifier("FullName")]
        public string Name { get; set; }

        [CollectiveIdentifier("FullName","FamilyJob")]
        public string Surname { get; set; }

        public int Age { get; set; }

        [CollectiveIdentifier("FamilyJob")]
        public long JobId { get; set; }

        public Job Job { get; set; }

        public List<Address> Addresses { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}