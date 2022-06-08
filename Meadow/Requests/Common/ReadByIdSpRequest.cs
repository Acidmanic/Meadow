using Meadow.Reflection.Conventions;

namespace Meadow.Requests.Common
{
    public abstract class ReadByIdSpRequest<TModel> : MeadowRequest<MeadowVoid, TModel>
        where TModel : class, new()
    {
        private readonly bool _fullTree;


        protected ReadByIdSpRequest(bool fullTree) : base(true)
        {
            _fullTree = fullTree;
        }

        public ReadByIdSpRequest() : this(false)
        {
        }

        protected override string GetRequestText()
        {
            var naming = new NameConvention(typeof(TModel));

            return _fullTree ? naming.SelectByIdProcedureNameFullTree : naming.SelectByIdProcedureName;
        }
    }
}