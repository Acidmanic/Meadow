using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.DataTypeMapping.Attributes;

namespace Example.SqlServer.Models
{
    public class Person
    {
        [UniqueMember]
        [AutoValuedMember]
        public long Id { get; set; }

        [ForceColumnSize(64)]
        public string Name { get; set; }

        [ForceColumnSize(128)]
        public string Surname { get; set; }

        public int Age { get; set; }

        public long JobId { get; set; }

        public Job Job { get; set; }

        public List<Address> Addresses { get; set; }
    }
}