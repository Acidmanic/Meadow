using System;
using Acidmanic.Utilities.DataTypes;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Extensions;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests.Models;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed class
        PerformSearchIfNeededExtendedRequest<TStorage, TId> : MeadowRequest<FilterShellExtended, FilterResult<TId>>
    {
        public PerformSearchIfNeededExtendedRequest(FilterQuery filter, string[] searchTerms = null,
            string searchId = null) : base(true)
        {
            RegisterTranslationTask(t =>
            {
                searchTerms ??= new string[]{};

                var filterExpression = t.TranslateFilterQueryToDbExpression(filter, FullTreeReadWrite());

                var searchExpression = t.TranslateSearchTerm(typeof(TStorage), searchTerms);

                ToStorage = new FilterShellExtended
                {
                    FilterExpression = filterExpression,
                    SearchExpression = searchExpression,
                    SearchId = searchId ?? Guid.NewGuid().SearchId(),
                    ExpirationTimeStamp = typeof(TStorage).GetFilterResultExpirationPointMilliseconds()
                };
            });
        }

        public override string RequestText
        {
            get => (FullTreeReadWrite()
                ? Configuration.GetNameConvention<TStorage>().PerformFilterIfNeededProcedureNameFullTree
                : Configuration.GetNameConvention<TStorage>().PerformFilterIfNeededProcedureName) + "Extended";
            protected set { }
        }
    }
}