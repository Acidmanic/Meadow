using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Scaffolding.Models;

namespace Meadow.Requests.BuiltIn
{
    public sealed class SaveRequest<T> : MeadowRequest<T, T> where T : class
    {
        private readonly string _identifierCollectionName;
        public SaveRequest(T model, string? identifierCollectionName = null) : base(true)
        {
            _identifierCollectionName = identifierCollectionName ?? RecordIdentificationProfile.DefaultCollection;
            
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