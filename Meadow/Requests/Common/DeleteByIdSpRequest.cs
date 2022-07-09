using Meadow.Reflection.Conventions;
using Meadow.Requests.Common.Models;
using Meadow.Requests.FieldManipulation;

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