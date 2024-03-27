using Meadow.Extensions;

namespace Meadow.Requests.Common
{
    public abstract class UpdateSpRequest<TModel> : MeadowRequest<TModel>
    {
        protected UpdateSpRequest(TModel value) : base(value)
        {
        }
        
        public override string RequestText => Convention<TModel>().UpdateProcedureName;
    }
}