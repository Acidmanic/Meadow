using System;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Contracts;
using Meadow.Extensions;

namespace Meadow.Requests.BuiltIn
{
    public record FindPagedShell(long Offset, long Size, string FilterExpression, string SearchExpression,
        string OrderExpression);

    public sealed class FindPagedRequest<TStorage> : MeadowRequest<FindPagedShell,TStorage > where TStorage : class
    {
        public FindPagedRequest(
            FilterQuery filter,
            long offset = 0,
            long size = 100,
            string[]? searchTerms = null,
            OrderTerm[]? orders = null) : base(true)
        {
            RegisterTranslationTask(t =>
            {
                var filterExpression = t.TranslateFilterQueryToDbExpression(filter,
                    FullTreeReadWrite() ? ColumnNameTranslation.FullTree : ColumnNameTranslation.ColumnNameOnly);

                searchTerms ??= new string[] { };

                searchTerms = searchTerms.Select(Configuration.TransliterationService.Transliterate).ToArray();

                var searchExpression = t.TranslateSearchTerm(typeof(TStorage), searchTerms);

                orders ??= new OrderTerm[] { };

                var ordersExpression = t.TranslateOrders(typeof(TStorage), orders, FullTreeReadWrite());

                ToStorage = new FindPagedShell(offset,size,filterExpression,searchExpression,ordersExpression);
            });
        }

        public override string RequestText
        {
            get => FullTreeReadWrite()
                ? Configuration.GetNameConvention<TStorage>().FindPagedProcedureNameFullTree
                : Configuration.GetNameConvention<TStorage>().FindPagedProcedureName;
            protected set { }
        }
    }
}