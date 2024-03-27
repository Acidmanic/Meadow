using Meadow.Extensions;

namespace Meadow.Requests.Common
{
    public abstract class ReadAllSpRequest<TModel> : MeadowRequest<TModel>
    {
        public override string RequestText => Convention<TModel>().SelectAllProcedureName;
    }
}