namespace Meadow.Requests.Common
{
    public abstract class ReadAllSpRequest<TModel> : MeadowRequest<MeadowVoid, TModel>
        where TModel : class, new()
    {
        protected ReadAllSpRequest() : base(true)
        {
        }
    }
}