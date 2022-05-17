using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.Requests
{
    public class GetAllPersons : MeadowRequest<MeadowVoid, Person>
    {
        public GetAllPersons() : base(true)
        {
        }
    }
}