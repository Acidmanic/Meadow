using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Contracts;
using Meadow.Requests;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Test.Functional.TestCaseClasses;
using Newtonsoft.Json;

namespace Meadow.Test.Functional
{
    public class Tdd38MeadowFiltering : MeadowFunctionalTest
    {
        private class ReadAllPersonsRequest : MeadowRequest<MeadowVoid, Person>
        {
            public ReadAllPersonsRequest() : base(true)
            {
            }
        }

        private class Filter
        {
            public string FilterHash { get; set; }

            public long ExpirationTimeStamp { get; set; }

            public string WhereClause { get; set; }
        }

        private class FilterChunk
        {
            public long Offset { get; set; }

            public long Size { get; set; }

            public string FilterHash { get; set; }
        }

        private sealed class PerformPersonsFilterIfNeededRequest : MeadowRequest<Filter, FilterResult<long>>
        {
            public PerformPersonsFilterIfNeededRequest(FilterQuery filterQuery) : base(true)
            {
                RegisterTranslationTask(tr =>
                {
                    ToStorage = new Filter
                    {
                        FilterHash = filterQuery.Hash(),
                        ExpirationTimeStamp = TimeStamp.Now.TotalMilliSeconds +
                                              typeof(Person).GetFilterResultExpirationDurationMilliseconds(),
                        WhereClause = tr.TranslateFilterQueryToDbExpression(filterQuery,FullTreeReadWrite()?ColumnNameTranslation.FullTree:ColumnNameTranslation.ColumnNameOnly)
                    };
                });
            }
        }

        private sealed class ReadPersonsChunkRequest : MeadowRequest<FilterChunk, Person>
        {
            public ReadPersonsChunkRequest(long offset, long size, string filterHash) : base(true)
            {
                ToStorage = new FilterChunk
                {
                    Offset = offset,
                    Size = size,
                    FilterHash = filterHash
                };
            }
        }

        public override void Main()
        {
            UseSqlServer();

            var engine = CreateEngine();

            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }

            engine.CreateDatabase();

            engine.BuildUpDatabase();


            var allPersons = engine.PerformRequest(new ReadAllPersonsRequest())
                .FromStorage;

            var filter = new FilterQuery();

            filter.EntityType = typeof(Person);

            filter.Add(new FilterItem
            {
                Key = "Name",
                EqualityValues = new List<object> { "Mani", "Mona" },
                ValueType = typeof(string),
                ValueComparison = ValueComparison.Equal
            });

            var filterRequest = new PerformPersonsFilterIfNeededRequest(filter);

            var allSearchResults = engine.PerformRequest(filterRequest).FromStorage;

            var pagination = new { Offset = 0, Size = 20 };

            var chunkRequest = new ReadPersonsChunkRequest(pagination.Offset, pagination.Size, filter.Hash());

            var filteringResults = engine.PerformRequest(chunkRequest).FromStorage;

            var paginatedData = new
            {
                Offset = pagination.Offset,
                Size = pagination.Size,
                TotalResults = allSearchResults.Count,
                Results = filteringResults
            };
        }
    }
}