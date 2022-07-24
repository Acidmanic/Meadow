using Example.SqLite.Models;
using Meadow;
using Meadow.Requests;

namespace Example.SqLite.Requests
{
    public class GetAllPersonsFullTreeRequest:MeadowRequest<MeadowVoid,Person>
    {
        public GetAllPersonsFullTreeRequest() : base(true)
        {
        }
    }
}