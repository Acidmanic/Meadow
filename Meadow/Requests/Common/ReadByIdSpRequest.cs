namespace Meadow.Requests.Common
{
    public abstract class ReadByIdSpRequest<TModel, TId> : ByIdRequestBase<TModel, TId, TModel>
        where TModel : class, new()
    {
        private readonly bool _fullTree;

        protected ReadByIdSpRequest(bool fullTree) : base(true, fullTree)
        {
            _fullTree = fullTree;
        }

        public ReadByIdSpRequest() : this(false)
        {
        }

    }
}