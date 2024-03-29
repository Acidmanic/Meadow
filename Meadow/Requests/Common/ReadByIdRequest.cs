using Meadow.Extensions;

namespace Meadow.Requests.Common
{
    public class ReadByIdRequest<TModel, TId> : MeadowRequest<TModel>
    {
        public ReadByIdRequest(TId id) : base(typeof(TModel).CreateIdShellFor(id))
        {
        }
        
        public override string RequestText => Convention<TModel>().SelectByIdProcedureName;
    }
    
    public class ReadByIdRequest<TModel> : MeadowRequest<TModel>
    {
        public ReadByIdRequest(object id) : base(typeof(TModel).CreateIdShellFor(id))
        {
        }
        
        public override string RequestText => Convention<TModel>().SelectByIdProcedureName;
    }
}