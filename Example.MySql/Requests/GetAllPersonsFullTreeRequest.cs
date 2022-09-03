using Example.MySql.Models;
using Meadow;
using Meadow.Requests;

namespace Example.MySql.Requests
{
    public class GetAllPersonsFullTreeRequest:MeadowRequest<MeadowVoid,Person>
    {
        public GetAllPersonsFullTreeRequest() : base(true)
        {
        }

        protected override bool FullTreeReadWrite()
        {
            return true;
        }
    }
}