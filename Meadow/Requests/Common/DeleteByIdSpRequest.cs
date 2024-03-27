using Meadow.Requests.Common.Models;

namespace Meadow.Requests.Common
{
    public abstract class DeleteByIdSpRequest<TModel,TId> : MeadowRequest<DeletionResult>
    {
    
        protected DeleteByIdSpRequest(TId id) : base(typeof(TModel).CreateIdShellFor(id))
        {
        }
       
        public override string RequestText => Convention<TModel>().DeleteByIdProcedureName;
    }
}