using Meadow.Extensions;
using Meadow.Requests;

namespace Meadow.Test.Functional.GenericRequests
{
    public class ReadAllRequest<T> : MeadowRequest<MeadowVoid, T> where T : class, new()
    {
        public ReadAllRequest() : base(true)
        {
        }

        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<T>();

                if (FullTreeReadWrite())
                {
                    return nc.SelectAllProcedureNameFullTree;
                }

                return nc.SelectAllProcedureName;
            }
        }
    }
}