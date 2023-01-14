using Acidmanic.Utilities.Reflection.FieldInclusion;
using Example.Async.SqlServer.Models;
using Meadow.Requests;

namespace Example.Async.SqlServer.Requests
{
    public sealed class InsertJobRequest : MeadowRequest<Job, Job>
    {
        public InsertJobRequest(Job job) : base(true)
        {
            ToStorage = job;
        }

        protected override void OnFieldManipulation(IFieldInclusionMarker<Job> toStorage,
            IFieldInclusionMarker<Job> fromStorage)
        {
            base.OnFieldManipulation(toStorage, fromStorage);

            toStorage.Exclude(j => j.Id);
        }
    }
}