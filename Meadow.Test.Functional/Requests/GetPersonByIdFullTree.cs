using Meadow.Requests;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.Requests
{
    public class GetPersonByIdFullTree : MeadowRequest<Person>
    {
        public GetPersonByIdFullTree(long id) : base(new {Id = id})
        {
            
        }
    }
}