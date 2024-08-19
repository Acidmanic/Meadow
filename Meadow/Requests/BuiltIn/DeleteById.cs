using Meadow.Extensions;
using Meadow.Requests.BuiltIn.Dtos;

namespace Meadow.Requests.BuiltIn
{
    public sealed class DeleteById<TEntity,TId>:MeadowRequest<IdDto<TId>,SuccessDto>
    {
        public DeleteById(TId id) : base(true)
        {
            ToStorage = id;
        }

        public override string RequestText { 
            get => Configuration.GetNameConvention<TEntity>().DeleteByIdProcedureName;
            protected set
            {
                
            }
        }
    }
}