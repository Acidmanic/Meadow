using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Meadow.Extensions;
using Meadow.Requests;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed class SaveRequest<T> : MeadowRequest<T, T> where T : class, new()
    {
        public SaveRequest(T model) : base(true)
        {
            ToStorage = model;
        }

        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<T>();

                return nc.SaveProcedureName;
            }
        }
    }
}