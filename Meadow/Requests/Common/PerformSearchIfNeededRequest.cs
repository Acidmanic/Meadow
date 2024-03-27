using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Extensions;
using Meadow.Requests.Common.Models;

namespace Meadow.Requests.Common
{
    public sealed class PerformSearchIfNeededRequest<TStorage> : MeadowRequest<FilterResponse>
    {
        public PerformSearchIfNeededRequest(
            FilterQuery filter,
            string? searchId = null,
            string[]? searchTerms = null,
            OrderTerm[]? orders = null) : base(true)
        {
            RegisterTranslationTask(t =>
            {
                var filterExpression = t.TranslateFilterQueryToDbExpression(filter, false);

                searchTerms ??= new string[] { };

                var searchExpression = t.TranslateSearchTerm(typeof(TStorage), searchTerms);

                orders ??= new OrderTerm[] { };

                var ordersExpression = t.TranslateOrders(typeof(TStorage), orders, false);

                SetToStorage(
                    new
                    {
                        FilterExpression = filterExpression,
                        SearchExpression = searchExpression,
                        SearchId = searchId ?? Guid.NewGuid().SearchId(),
                        OrderExpression = ordersExpression,
                        ExpirationTimeStamp = typeof(TStorage).GetFilterResultExpirationPointMilliseconds()
                    });
            });
        }

        public override string RequestText => Convention<TStorage>().PerformFilterIfNeededProcedureName;
    }
}