using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests;

namespace Meadow.Configuration.Requests
{
    public class ReadLastModel<TModel> : MeadowRequest<TModel> where TModel : class, new()
    {
        public override string RequestText => Configuration.GetNameConvention<TModel>().SelectLastProcedureName;
    }
}