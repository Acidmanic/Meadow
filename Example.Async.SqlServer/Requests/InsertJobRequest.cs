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

        protected override void OnFieldManipulation(IFieldInclusionMarker toStorage, IFieldInclusionMarker fromStorage)
        {
            base.OnFieldManipulation(toStorage, fromStorage);

            toStorage.Exclude((Job j) => j.Id);
        }
    }
}