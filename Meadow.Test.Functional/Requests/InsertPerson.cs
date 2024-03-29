using Meadow.Requests.Common;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.Requests
{
    public class InsertPerson : InsertRequest<Person>
    {
        public InsertPerson(Person person):base(person)
        {
            
        }   
    }
}