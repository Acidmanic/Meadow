using Meadow.Requests.Common.Models;

namespace Meadow.Requests.Common
{
    public class DeleteByIdRequest<TModel,TId> : MeadowRequest<DeletionResult>
    {
    
        public DeleteByIdRequest(TId id) : base(typeof(TModel).CreateIdShellFor(id))
        {
        }
       
        public override string RequestText => Convention<TModel>().DeleteByIdProcedureName;
    }
    public class DeleteByIdRequest<TModel> : MeadowRequest<DeletionResult>
    {
    
        public DeleteByIdRequest(object id) : base(typeof(TModel).CreateIdShellFor(id))
        {
        }
       
        public override string RequestText => Convention<TModel>().DeleteByIdProcedureName;
    }
}