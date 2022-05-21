using Meadow.Requests.Common;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.Requests
{
    public class GetAllPersonsFullTree : ReadAllSpRequest<Person>
    {
        protected override bool FullTreeReadWrite()
        {
            return true;
        }
    }
}