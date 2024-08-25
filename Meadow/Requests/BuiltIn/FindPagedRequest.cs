using System;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests.BuiltIn.Dtos;

namespace Meadow.Requests.BuiltIn
{
    public sealed class FindPagedRequest<TStorage> : MeadowRequest<FindPagedDto, TStorage> where TStorage : class
    {
        public FindPagedRequest(
            FilterQuery filter,
            long offset = 0,
            long size = 100,
            string[]? searchTerms = null,
            OrderTerm[]? orders = null) : base(true)
        {
            Setup(context =>
            {
                var filterExpression = context.SqlTranslator.TranslateFilterQueryToDbExpression(filter,
                    FullTreeReadWrite() ? ColumnNameTranslation.FullTree : ColumnNameTranslation.ColumnNameOnly);

                searchTerms ??= new string[] { };

                searchTerms = searchTerms.Select(context.Transliterator.Transliterate).ToArray();

                var searchExpression = context.SqlTranslator.TranslateSearchTerm(typeof(TStorage), searchTerms);

                orders ??= new OrderTerm[] { };

                var ordersExpression = context.SqlTranslator.TranslateOrders(typeof(TStorage), orders, FullTreeReadWrite());

                ToStorage = new FindPagedDto(offset, size, filterExpression, searchExpression, ordersExpression);
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