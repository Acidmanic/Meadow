namespace Meadow.Requests.Common
{
    public abstract class InsertSpRequest<TModel> : MeadowRequest<TModel, TModel>
        where TModel : class, new()
    {
        protected InsertSpRequest() : base(true)
        {
        }

        protected override void OnExclusion(FieldExclusionMarker<TModel> exclusionMarker)
        {
            exclusionMarker.Exclude("Id");
        }
    }
}