using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using Example.Filtering.Models;
using Example.Filtering.Requests.Models;
using Meadow.Extensions;
using Meadow.Requests;

namespace Example.Filtering.Requests
{
    public sealed class PerformPersonsFilterIfNeededRequest : MeadowRequest<FilterShell, FilterResult>
    {
        public PerformPersonsFilterIfNeededRequest(FilterQuery filterQuery,string searchId = null) : base(true)
        {

            searchId ??= Guid.NewGuid().SearchId();
            
            RegisterTranslationTask(t =>
            {
                ToStorage = new FilterShell
                {
                    FilterExpression = t.TranslateFilterQueryToWhereClause(filterQuery,FullTreeReadWrite()),
                    ExpirationTimeStamp = typeof(Person).GetFilterResultExpirationPointMilliseconds(),
                    SearchId = searchId
                };
            });
        }
    }
}