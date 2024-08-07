using Acidmanic.Utilities.Reflection.FieldInclusion;
using Example.Filtering.Models;
using Meadow.Requests;

namespace Example.Filtering.Requests
{
    public sealed class InsertPersonRequest:MeadowRequest<Person,Person>
    {
        public InsertPersonRequest(Person person) : base(true)
        {
            ToStorage = person;
        }

        protected override void OnFieldManipulation(IFieldInclusionMarker toStorage, IFieldInclusionMarker fromStorage)
        {
            base.OnFieldManipulation(toStorage, fromStorage);

            toStorage.Exclude((Person p) => p.Id);
        }
    }
}