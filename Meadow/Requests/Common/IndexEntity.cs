using Acidmanic.Utilities.Filtering.Models;
using Meadow.Extensions;

namespace Meadow.Requests.Common
{
    public sealed class IndexEntity<TEntity, TId> : MeadowRequest<SearchIndex<TId>>
    {
        public IndexEntity(string corpus, TId id) : base(new SearchIndex<TId>()
        {
            IndexCorpus = corpus,
            ResultId = id
        })
        {
            typeof(TEntity).IfHasIdField(key => InputFields.Exclude(key));
        }

        public override string RequestText => Configuration.GetNameConvention<TEntity>().IndexEntityProcedureName;
    }
}