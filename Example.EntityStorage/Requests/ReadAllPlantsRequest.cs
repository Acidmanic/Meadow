using Example.EntityStorage.Entities;
using Meadow;
using Meadow.Requests;

namespace Example.EntityStorage.Requests
{
    public class ReadAllPlantsRequest:MeadowRequest<MeadowVoid,Plant>
    {
        public ReadAllPlantsRequest() : base(true)
        {
        }

        protected override bool FullTreeReadWrite()
        {
            return true;
        }
    }
}