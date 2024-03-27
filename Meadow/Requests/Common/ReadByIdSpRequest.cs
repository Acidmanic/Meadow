using Meadow.Extensions;

namespace Meadow.Requests.Common
{
    public abstract class ReadByIdSpRequest<TModel, TId> : MeadowRequest<TModel>
    {
        protected ReadByIdSpRequest(TId id) : base(typeof(TModel).CreateIdShellFor(id))
        {
        }
        
        public override string RequestText => Convention<TModel>().SelectByIdProcedureName;
    }
}