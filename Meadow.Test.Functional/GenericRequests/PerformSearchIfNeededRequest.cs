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
    public sealed class PerformSearchIfNeededRequest<TStorage> : MeadowRequest<FilterShell, FilterResult>
    {
        public PerformSearchIfNeededRequest(FilterQuery filter,string searchId = null) : base(true)
        {
            RegisterTranslationTask(t =>
            {
                ToStorage = new FilterShell
                {
                    FilterExpression = t.TranslateFilterQueryToWhereClause(filter, FullTreeReadWrite()),
                    SearchId = searchId ?? Guid.NewGuid().SearchId(),
                    ExpirationTimeStamp = typeof(TStorage).GetFilterResultExpirationPointMilliseconds()
                };
            });
        }

        public override string RequestText
        {
            get => Configuration.GetNameConvention<TStorage>().PerformFilterIfNeededProcedureName;
            protected set
            {
                    
            }
        }
    }
}