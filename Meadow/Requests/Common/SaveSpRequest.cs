namespace Meadow.Requests.Common
{
    public abstract class SaveSpRequest<TModel> : MeadowRequest<TModel, TModel>
        where TModel : class, new()
    {
        protected SaveSpRequest() : base(true)
        {
        }
    }
}