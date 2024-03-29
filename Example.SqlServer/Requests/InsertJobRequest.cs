using Acidmanic.Utilities.Reflection.FieldInclusion;
using Example.SqlServer.Models;
using Meadow.Requests;

namespace Example.SqlServer.Requests
{
    public sealed class InsertJobRequest : MeadowRequest<Job, Job>
    {
        public InsertJobRequest(Job job) : base(true)
        {
            ToStorage = job;
        }

     }
}