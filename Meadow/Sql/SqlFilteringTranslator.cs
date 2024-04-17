using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Results;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Inclusion.Enums;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.Translators;
using Microsoft.Extensions.Logging;

namespace Meadow.Sql
{
    internal sealed class SqlFilteringTranslator : ISqlFilteringTranslator
    {
        public SqlFilteringTranslator(ILogger logger, MeadowConfiguration configuration, ISqlLanguageTranslator translator)
        {
            Logger = logger;
            Configuration = configuration;
            Translator = translator;
        }

        public ILogger Logger { get;  }
        public MeadowConfiguration Configuration { get;  }
        
        public ISqlLanguageTranslator Translator { get;  }

        
        public string TranslateFilterQueryToDbExpression(FilterQuery filterQuery)
        {
            Func<FilterItem, Result<string>> pickColumName = item => new Result<string>(true, item.Key);

            if (fullTree)
            {
                var columns = Configuration.GetFullTreeMap(filterQuery.EntityType);

                pickColumName = item => columns.GetColumnName(item.Key);
            }

            var sb = new StringBuilder();

            var anyFilters = false;

            var sep = "";

            foreach (var filter in filterQuery.Items())
            {
                anyFilters = true;


                sb.Append(sep).Append("(");

                Append(sb, filter, pickColumName);

                sb.Append(")");

                sep = " AND ";
            }

            if (anyFilters)
            {
                return sb.ToString();
            }

            return Translator.EmptyConditionExpression;
        }

        public string TranslateSearchTerm(Type entityType, string[]? searchTerms)
        {
            if (searchTerms == null || searchTerms.Length == 0)
            {
                return Translator.EmptyConditionExpression;
            }

            var nc = Configuration.GetNameConvention(entityType);
            
            var tq = Translator.QuoteTableName;
            
            var cq = Translator.QuotesColumnName;

            var searchIndexTable = nc.SearchIndexTableName;

            var columnFullName = tq(searchIndexTable) + "." + cq("IndexCorpus");

            return string.Join(" OR ", searchTerms.Select(
                s => $"{columnFullName} like '%{s}%'"));
        }

        public string TranslateOrders(Type entityType, OrderTerm[]? orders)
        {
            if (orders == null || orders.Length == 0)
            {
                return Translator.EmptyOrderExpression(entityType);
            }

            var q = Translator.QuoteTableName;
            var map = Configuration.GetFullTreeMap(entityType);
            Func<OrderTerm, string> sort = o => o.Sort == OrderSort.Descending ? "DESC" : "ASC";
            Func<OrderTerm, string> column = o => q(TranslateFieldName(map, o.Key, fullTree));

            var terms = orders.Select(o => column(o) + " " + sort(o));

            return string.Join(", ", terms);
        }

        public string TranslateFieldName(Type ownerEntityType, string headlessAddress)
        {
            if (fullTree)
            {
                var map = Configuration.GetFullTreeMap(ownerEntityType);

                return TranslateFieldName(map, headlessAddress, true);
            }

            return headlessAddress;
        }

        private string TranslateFieldName(FullTreeMap map, string headlessAddress, bool fullTree)
        {
            if (fullTree)
            {
                var fieldName = map.GetColumnName(headlessAddress);

                if (fieldName)
                {
                    return fieldName.Value;
                }

                return headlessAddress;
            }

            return headlessAddress;
        }

        private void Append(StringBuilder sb, FilterItem filter, Func<FilterItem, Result<string>> pickKey)
        {
            var max = HandleQuotingAndEscaping(filter.Maximum, filter.ValueType);
            var min = HandleQuotingAndEscaping(filter.Minimum, filter.ValueType);
            var equalities = HandleQuotingAndEscaping(filter.EqualityValues, filter.ValueType);

            var foundKey = pickKey(filter);

            if (foundKey)
            {
                var columnName = Translator.QuotesColumnName(foundKey.Value);

                var sep = "";
                
                switch (filter.ValueComparison)
                {
                    case ValueComparison.SmallerThan:
                        sb.Append(columnName).Append('<').Append(max);
                        break;
                    case ValueComparison.LargerThan:
                        sb.Append(columnName).Append('>').Append(min);
                        break;
                    case ValueComparison.BetweenValues:
                        sb.Append(columnName).Append('<').Append(max)
                            .Append(" AND ")
                            .Append(columnName).Append('>').Append(min);
                        break;
                    case ValueComparison.Equal:
                        sep = "";
                        foreach (var equalValue in equalities)
                        {
                            sb.Append(sep).Append(columnName).Append('=').Append(equalValue);
                            sep = " OR ";
                        }

                        break;
                    case ValueComparison.NotEqual:
                        sep = "";
                        
                        foreach (var equalValue in equalities)
                        {
                            var ieqOpr = Translator.ComparisonOperator(Operators.IsNotEqualTo, filter.ValueType, filter.ValueType);
                            
                            sb.Append(sep).Append(columnName).Append(ieqOpr).Append(equalValue);
                            
                            sep = " AND ";
                        }

                        break;
                }
            }
        }
        

        private string HandleQuotingAndEscaping(string? value, Type type)
        {
            if (value == null)
            {
                return DefaultValue(type);
            }

            if (type == typeof(string))
            {
                var escaped = value.Replace("'", Translator.EscapedSingleQuote);

                return $"'{escaped}'";
            }

            return value;
        }

        private string DefaultValue(Type type)
        {
            if (TypeCheck.IsNumerical(type))
            {
                return "0";
            }

            if (type == typeof(string))
            {
                return "''";
            }

            return "";
        }

        private List<string> HandleQuotingAndEscaping(IEnumerable<string> values, Type type)
        {
            var items = new List<string>();

            foreach (var value in values)
            {
                items.Add(HandleQuotingAndEscaping(value, type));
            }

            return items;
        }
    }
}