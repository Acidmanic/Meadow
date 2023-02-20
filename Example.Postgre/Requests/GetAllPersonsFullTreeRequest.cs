using Example.Postgre.Models;
using Meadow;
using Meadow.Requests;

namespace Example.Postgre.Requests
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

        protected override bool QuoteProcedureName()
        {
            return true;
        }
    }
}