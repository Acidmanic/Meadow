using Meadow.Extensions;
using Meadow.Requests.BuiltIn.Dtos;

namespace Meadow.Requests.BuiltIn
{
    public sealed class ReadAllRequest<TEntity> : MeadowRequest<MeadowVoid, TEntity> where TEntity : class, new()
    {
        public ReadAllRequest() : base(true)
        {
        }

        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<TEntity>();

                return FullTreeReadWrite() ? nc.SelectAllProcedureNameFullTree : nc.SelectAllProcedureName;
            }
        }
    }
}