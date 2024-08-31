using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests;

namespace Meadow.Configuration.Requests
{
    public class ReadLastModel<TModel> : MeadowRequest<MeadowVoid, TModel> where TModel : class
    {
        public ReadLastModel() : base(true)
        {
        }


        public override string RequestText
        {
            get => Configuration.GetNameConvention<TModel>().SelectLastProcedureName;
            protected set { }
        }
    }
}