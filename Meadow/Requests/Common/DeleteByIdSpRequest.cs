using Meadow.Requests.Common.Models;

namespace Meadow.Requests.Common
{
    public abstract class DeleteByIdSpRequest<TModel,TId> : ByIdRequestBase<TModel,TId,DeletionResult>
        where TModel : class
    {
    
        protected DeleteByIdSpRequest() : base(true, false)
        {
        }
       
        
    }
}