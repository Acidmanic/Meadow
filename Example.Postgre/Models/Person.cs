using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.Postgre.Models
{
    public class Person
    {
        [UniqueMember]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public int Age { get; set; }

        public long JobId { get; set; }

        public Job Job { get; set; }

        public List<Address> Addresses { get; set; }
    }
}