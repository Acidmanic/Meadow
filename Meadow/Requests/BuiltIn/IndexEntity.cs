using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Meadow.Extensions;

namespace Meadow.Requests.BuiltIn
{
    public sealed class IndexEntity<TEntity,TId> : MeadowRequest<SearchIndex<TId>, SearchIndex<TId>> 
    {
        public IndexEntity(string corpus,TId id) : base(true)
        {
            ToStorage = new SearchIndex<TId>()
            {
                IndexCorpus = corpus,
                ResultId = id
            };
        }

        public IndexEntity(TEntity model, bool fullTree = true) : base(true)
        {
            
            Setup(c =>
            {
                var indexing = c.GetCorpusService<TEntity>();

                var corpus = indexing.GetIndexCorpus(model, fullTree);
            
                ToStorage = new SearchIndex<TId>()
                {
                    IndexCorpus = corpus,
                    ResultId = model.ReadIdOrDefault<TEntity,TId>()! 
                };    
                
            });
        }

        protected override void OnFieldManipulation(IFieldInclusionMarker toStorage, IFieldInclusionMarker fromStorage)
        {
            base.OnFieldManipulation(toStorage, fromStorage);
            
            var idLeaf = TypeIdentity.FindIdentityLeaf<SearchIndex<TId>>();
                
            toStorage.Exclude(idLeaf.GetFullName());
            
        }

        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<TEntity>();

                return nc.IndexEntityProcedureName;
            }
        }
    }
}