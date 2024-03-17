using Example.EntityStorage.Entities;
using Example.EntityStorage.ValueObjects;
using Meadow.Requests;

namespace Example.EntityStorage.Requests
{
    public sealed class InsertPlantTypeRequest:MeadowRequest<PlantType,PlantType>
    {
        public InsertPlantTypeRequest(PlantType plant) : base(true)
        {
            ToStorage = plant;
        }
    }
}