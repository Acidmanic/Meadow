namespace Meadow.Requests.Common
{
    public abstract class UpdateSpRequest<TModel> : MeadowRequest<TModel, TModel>
        where TModel : class
    {
        protected UpdateSpRequest() : base(true)
        {
        }
    }
}