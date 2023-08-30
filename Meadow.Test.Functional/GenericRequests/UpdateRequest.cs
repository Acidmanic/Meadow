using Meadow.Extensions;
using Meadow.Requests;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed class UpdateRequest<T> : MeadowRequest<T, T> where T : class, new()
    {
        public UpdateRequest(T model) : base(true)
        {
            ToStorage = model;
        }


        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<T>();

                return nc.UpdateProcedureName;
            }
        }
    }
}