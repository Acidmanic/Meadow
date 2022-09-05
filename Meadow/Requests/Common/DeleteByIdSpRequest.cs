using Meadow.Reflection.Conventions;
using Meadow.Requests.Common.Models;

namespace Meadow.Requests.Common
{
    public abstract class DeleteByIdSpRequest<TModel,TId> : ByIdRequestBase<TModel,TId,DeletionResult>
        where TModel : class, new()
    {
    
        protected DeleteByIdSpRequest() : base(true, false)
        {
        }
       
        
    }
}