using Example.EntityStorage.Entities;
using Meadow;
using Meadow.Requests;

namespace Example.EntityStorage.Requests
{
    public class ReadAllPlantsFullTreeRequest:MeadowRequest<MeadowVoid,Plant>
    {
        public ReadAllPlantsFullTreeRequest() : base(true)
        {
        }

        protected override bool FullTreeReadWrite()
        {
            return true;
        }
    }
}