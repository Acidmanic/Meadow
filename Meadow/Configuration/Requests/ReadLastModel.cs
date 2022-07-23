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


        public override string RequestText
        {
            get { return new NameConvention(typeof(TModel)).SelectLastProcedureName; }
            protected set { }
        }
    }
}