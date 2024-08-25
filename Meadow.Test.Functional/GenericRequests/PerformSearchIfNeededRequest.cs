using System;
using Acidmanic.Utilities.DataTypes;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests.Models;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed class PerformSearchIfNeededRequest<TStorage,TId> : MeadowRequest<FilterShell, FilterResponse>
    {
        public PerformSearchIfNeededRequest(
            FilterQuery filter,
            string searchId = null, 
            string[] searchTerms = null,
            OrderTerm[] orders = null) : base(true)
        {
            Setup(context =>
            {
                var filterExpression = context.SqlTranslator.TranslateFilterQueryToDbExpression(filter, FullTreeReadWrite()?
                    ColumnNameTranslation.FullTree:ColumnNameTranslation.ColumnNameOnly); 
                
                searchTerms ??= new string[]{};

                var searchExpression = context.SqlTranslator.TranslateSearchTerm(typeof(TStorage), searchTerms);

                orders ??= new OrderTerm[] { };
                
                var ordersExpression = context.SqlTranslator.TranslateOrders(typeof(TStorage), orders, FullTreeReadWrite());
                
                ToStorage = new FilterShell
                {
                    FilterExpression = filterExpression,
                    SearchExpression = searchExpression,
                    SearchId = searchId ?? Guid.NewGuid().SearchId(),
                    OrderExpression = ordersExpression,
                    ExpirationTimeStamp = typeof(TStorage).GetFilterResultExpirationPointMilliseconds()
                };
            });

        }

        public override string RequestText
        {
            get => FullTreeReadWrite()?Configuration.GetNameConvention<TStorage>().PerformFilterIfNeededProcedureNameFullTree: 
                Configuration.GetNameConvention<TStorage>().PerformFilterIfNeededProcedureName;
            protected set
            {
                    
            }
        }
    }
}