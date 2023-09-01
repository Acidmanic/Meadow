using Meadow.Extensions;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests.Models;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed class ReadByIdRequest<TEntity,TId> : MeadowRequest<IdShell<TId>, TEntity> where TEntity : class, new()
    {
        public ReadByIdRequest(TId id) : base(true)
        {
            ToStorage = new IdShell<TId>
            {
                Id = id
            };
        }

        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<TEntity>();

                return FullTreeReadWrite()?nc.SelectByIdProcedureNameFullTree: nc.SelectByIdProcedureName;
            }
        }
    }
}