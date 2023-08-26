using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using Example.Filtering.Models;
using Example.Filtering.Requests.Models;
using Meadow.Requests;

namespace Example.Filtering.Requests
{
    public sealed class PerformPersonsFilterIfNeededRequest : MeadowRequest<FilterShell, FilterResult>
    {
        public PerformPersonsFilterIfNeededRequest(FilterQuery filterQuery) : base(true)
        {
            RegisterTranslationTask(t =>
            {
                ToStorage = new FilterShell
                {
                    FilterHash = filterQuery.Hash(),
                    FilterExpression = t.TranslateFilterQueryToWhereClause(filterQuery),
                    ExpirationTimeStamp = DateTime.Now.Ticks + typeof(Person).GetFilterResultExpirationTimeSpan().Ticks
                };
            });
        }
    }
}