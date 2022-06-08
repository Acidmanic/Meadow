using Meadow.Configuration.ConfigurationRequests.Models;
using Meadow.Reflection.Conventions;
using Meadow.Requests.Common.Models;
using Meadow.Requests.FieldManipulation;

namespace Meadow.Requests.Common
{
    public abstract class DeleteByIdSpRequest<TModel,TId> : MeadowRequest<TId, DeletionResult>
        where TModel : class, new()
    {
        protected DeleteByIdSpRequest() : base(true)
        {
        }

       
        protected override string GetRequestText()
        {
            return new NameConvention(typeof(TModel)).DeleteByIdProcedureName;
        }
    }
}