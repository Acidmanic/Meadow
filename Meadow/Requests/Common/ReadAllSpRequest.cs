namespace Meadow.Requests.Common
{
    public abstract class ReadAllSpRequest<TModel> : MeadowRequest<MeadowVoid, TModel>
        where TModel : class
    {
        private readonly bool _fullTree;

        protected ReadAllSpRequest(bool fullTree) : base(true)
        {
            _fullTree = fullTree;
        }

        public ReadAllSpRequest() : this(false)
        {
        }

    }
}