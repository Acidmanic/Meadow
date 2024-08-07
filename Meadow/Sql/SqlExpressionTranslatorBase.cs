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
using Meadow.RelationalStandardMapping;
using Microsoft.Extensions.Logging;

namespace Meadow.Sql
{
    public abstract class SqlExpressionTranslatorBase : ISqlExpressionTranslator
    {
        public ILogger Logger { get; set; }
        public MeadowConfiguration Configuration { get; set; }

        public string TranslateFilterQueryToDbExpression(FilterQuery filterQuery, ColumnNameTranslation translation)
        {
            Func<FilterItem, Result<string>> pickColumName = item => new Result<string>(true, item.Key);

            if (translation==ColumnNameTranslation.FullTree)
            {
                var columns = Configuration.GetFullTreeMap(filterQuery.EntityType);

                pickColumName = item => columns.GetColumnName(item.Key);
                
            }else if (translation == ColumnNameTranslation.DataOwnerDotColumnName)
            {
                pickColumName = item => new Result<string>(true,
                    Configuration.TableNameProvider.GetNameForOwnerType(filterQuery.EntityType) +"."+item.Key);
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

            return EmptyConditionExpression;
        }

        public string TranslateSearchTerm(Type entityType, string[] searchTerms)
        {
            if (searchTerms == null || searchTerms.Length == 0)
            {
                return EmptyConditionExpression;
            }

            var nc = Configuration.GetNameConvention(entityType);

            Func<string, string> tq = DoubleQuotesTableNames ? s => $"\"{s}\"" : s => s;
            Func<string, string> cq = DoubleQuotesColumnNames ? s => $"\"{s}\"" : s => s;

            var searchIndexTable = nc.SearchIndexTableName;

            var columnFullName = tq(searchIndexTable) + "." + cq("IndexCorpus");

            return string.Join(" OR ", searchTerms.Select(
                s => $"{columnFullName} like '%{s}%'"));
        }

        public string TranslateOrders(Type entityType, OrderTerm[] orders, bool fullTree)
        {
            if (orders == null || orders.Length == 0)
            {
                return EmptyOrderExpression(entityType, fullTree);
            }

            Func<string, string> q = DoubleQuotesColumnNames ? s => $"\"{s}\"" : s => s;
            var map = Configuration.GetFullTreeMap(entityType);
            Func<OrderTerm, string> sort = o => o.Sort == OrderSort.Descending ? "DESC" : "ASC";
            Func<OrderTerm, string> column = o => q(TranslateFieldName(map, o.Key, fullTree));

            var terms = orders.Select(o => column(o) + " " + sort(o));

            return string.Join(", ", terms);
        }

        public string TranslateFieldName(Type ownerEntityType, string headlessAddress, bool fullTree)
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
                var columnName = DoubleQuotesColumnNames ? $"\"{foundKey.Value}\"" : foundKey.Value;

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
                            sb.Append(sep).Append(columnName).Append(NotEqualOperator).Append(equalValue);
                            sep = " AND ";
                        }

                        break;
                }
            }
        }


        protected abstract string EscapedSingleQuote { get; }

        protected abstract bool DoubleQuotesColumnNames { get; }

        protected abstract bool DoubleQuotesTableNames { get; }

        protected virtual string EmptyConditionExpression => "";
        
        protected virtual string NotEqualOperator => "!=";

        protected virtual string EmptyOrderExpression(Type entityType, bool fullTree)
        {
            return "";
        }

        protected virtual string HandleQuotingAndEscaping(string value, Type type)
        {
            if (value == null)
            {
                return DefaultValue(type);
            }

            if (type == typeof(string))
            {
                var escaped = value.Replace("'", EscapedSingleQuote);

                return $"'{escaped}'";
            }

            return value;
        }

        protected string DefaultValue(Type type)
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