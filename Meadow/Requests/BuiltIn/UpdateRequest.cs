using Meadow.Extensions;

namespace Meadow.Requests.BuiltIn
{
    public sealed class UpdateRequest<T> : MeadowRequest<T, T> where T : class
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