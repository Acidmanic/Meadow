using Example.AlteredType.Models;
using Meadow;
using Meadow.Requests;

namespace Example.AlteredType.Requests
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