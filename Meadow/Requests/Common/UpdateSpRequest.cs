using Meadow.Reflection.Conventions;

namespace Meadow.Requests.Common
{
    public abstract class UpdateSpRequest<TModel> : MeadowRequest<TModel, TModel>
        where TModel : class, new()
    {
        protected UpdateSpRequest() : base(false)
        {
        }
    }
}