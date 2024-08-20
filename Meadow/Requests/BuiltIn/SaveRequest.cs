using Meadow.Contracts;
using Meadow.Extensions;

namespace Meadow.Requests.BuiltIn
{
    public sealed class SaveRequest<T> : MeadowRequest<T, T> where T : class, new()
    {
        private readonly string _identifierCollectionName;
        public SaveRequest(T model, string? identifierCollectionName = null) : base(true)
        {
            _identifierCollectionName = identifierCollectionName ?? NameConvention.DefaultSaveCollectionName;
            
            ToStorage = model;
        }

        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<T>();

                return nc.GetSaveProcedureName(_identifierCollectionName);
            }
        }
    }
}