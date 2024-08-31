using Meadow.Extensions;
using Meadow.Requests.BuiltIn.Dtos;

namespace Meadow.Requests.BuiltIn
{
    public sealed class ReadAllRequest<TEntity> : MeadowRequest<MeadowVoid, TEntity> where TEntity : class
    {
        public ReadAllRequest() : base(true)
        {
        }

        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<TEntity>();

                return FullTreeReadWrite() ? nc.ReadAllProcedureNameFullTree : nc.ReadAllProcedureName;
            }
        }
    }
}