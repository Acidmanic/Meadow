using Example.AlteredType.Models;
using Meadow.Requests;

namespace Example.AlteredType.Requests
{
    public sealed class InsertPersonRequest:MeadowRequest<Person,Person>
    {
        public InsertPersonRequest(Person person) : base(true)
        {
            ToStorage = person;
        }
    }
}