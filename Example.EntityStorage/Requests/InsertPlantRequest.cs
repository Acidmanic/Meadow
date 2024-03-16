using Example.EntityStorage.Entities;
using Meadow.Requests;

namespace Example.EntityStorage.Requests
{
    public sealed class InsertPlantRequest:MeadowRequest<Plant,Plant>
    {
        public InsertPlantRequest(Plant plant) : base(true)
        {
            ToStorage = plant;
        }
    }
}