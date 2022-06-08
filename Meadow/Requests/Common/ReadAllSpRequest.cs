using Meadow.Reflection.Conventions;

namespace Meadow.Requests.Common
{
    public abstract class ReadAllSpRequest<TModel> : MeadowRequest<MeadowVoid, TModel>
        where TModel : class, new()
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