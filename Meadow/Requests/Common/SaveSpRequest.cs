using Meadow.Extensions;

namespace Meadow.Requests.Common
{
    public abstract class SaveSpRequest<TModel> : MeadowRequest<TModel>
    {
        protected SaveSpRequest(TModel value) : base(value)
        {
        }
        
        public override string RequestText => Convention<TModel>().SaveProcedureName;
    }
}