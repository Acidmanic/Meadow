using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using Example.Filtering.Models;
using Example.Filtering.Requests.Models;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests;

namespace Example.Filtering.Requests
{
    public sealed class PerformPersonsFilterIfNeededRequest<TEntity,TId> : MeadowRequest<FilterShell, FilterResult<TId>>
    {
        public PerformPersonsFilterIfNeededRequest(FilterQuery filterQuery,string searchId = null) : base(true)
        {

            searchId ??= Guid.NewGuid().SearchId();
            
            RegisterTranslationTask(t =>
            {
                ToStorage = new FilterShell
                {
                    FilterExpression = t.TranslateFilterQueryToDbExpression(filterQuery,FullTreeReadWrite()?ColumnNameTranslation.FullTree:ColumnNameTranslation.ColumnNameOnly),
                    ExpirationTimeStamp = typeof(TEntity).GetFilterResultExpirationPointMilliseconds(),
                    SearchId = searchId
                };
            });
        }
    }
}