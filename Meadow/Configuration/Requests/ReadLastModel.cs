using Meadow.BuildupScripts;
using Meadow.Reflection.Conventions;
using Meadow.Requests;

namespace Meadow.Configuration.ConfigurationRequests
{
    public class ReadLastModel<TModel> : MeadowRequest<MeadowVoid, TModel> where TModel : class, new()
    {
        public ReadLastModel() : base(true)
        {
        }


        protected override string GetRequestText()
        {
            return new NameConvention(typeof(TModel)).SelectLastProcedureName;
        }
    }
}