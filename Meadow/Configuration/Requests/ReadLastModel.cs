using Meadow.Reflection.Conventions;
using Meadow.Requests;

namespace Meadow.Configuration.Requests
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