using Meadow.Extensions;
using Meadow.Requests.BuiltIn.Dtos;

namespace Meadow.Requests.BuiltIn
{

    public sealed class ReadByIdRequest<TEntity, TId> : MeadowRequest<IdDto<TId>, TEntity> where TEntity : class, new()
    {
        public ReadByIdRequest(TId id) : base(true)
        {
            ToStorage = id;
        }

        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<TEntity>();

                return FullTreeReadWrite() ? nc.ReadByIdProcedureNameFullTree : nc.ReadByIdProcedureName;
            }
        }
    }
}