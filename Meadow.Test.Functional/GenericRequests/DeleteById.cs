using Meadow.Extensions;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests.Models;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed class DeleteById<TEntity,TId>:MeadowRequest<IdShell<TId>,SuccessResult>
    {
        public DeleteById(TId id) : base(true)
        {
            ToStorage = new IdShell<TId>
            {
                Id = id
            };
        }

        public override string RequestText { 
            get => Configuration.GetNameConvention<TEntity>().DeleteByIdProcedureName;
            protected set
            {
                
            }
        }
    }
}