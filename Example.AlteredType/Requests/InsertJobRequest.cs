using Example.AlteredType.Models;
using Meadow.Requests;

namespace Example.AlteredType.Requests
{
    public sealed class InsertJobRequest:MeadowRequest<Job,Job>
    {
        public InsertJobRequest(Job job) : base(true)
        {
            ToStorage = job;
        }
    }
}