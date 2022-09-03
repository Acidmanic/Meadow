using Example.SqlServer.Models;
using Meadow;
using Meadow.Requests;

namespace Example.SqlServer.Requests
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