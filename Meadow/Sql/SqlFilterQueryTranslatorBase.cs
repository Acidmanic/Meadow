using System;
using System.Collections.Generic;
using System.Text;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Results;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.RelationalStandardMapping;
using Microsoft.Extensions.Logging;

namespace Meadow.Sql
{
    public abstract class SqlFilterQueryTranslatorBase : IFilterQueryTranslator
    {
        public ILogger Logger { get; set; }
        public MeadowConfiguration Configuration { get; set; }
        
        public IDataOwnerNameProvider DataOwnerNameProvider { get; set; }

        public string TranslateFilterQueryToWhereClause(FilterQuery filterQuery, bool fullTree)
        {
            
            Func<FilterItem, Result<string>> pickColumName = item => new Result<string>(true, item.Key);

            if (fullTree)
            {
                var columns = new FullTreeMap
                (filterQuery.EntityType,
                    Configuration.DatabaseFieldNameDelimiter,
                    DataOwnerNameProvider);
                
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

            return "";
        }

        private void Append(StringBuilder sb, FilterItem filter, Func<FilterItem, Result<string>> pickKey)
        {
            var max = HandleQuotingAndEscaping(filter.Maximum, filter.ValueType);
            var min = HandleQuotingAndEscaping(filter.Minimum, filter.ValueType);
            var equals = HandleQuotingAndEscaping(filter.EqualValues, filter.ValueType);

            var foundKey = pickKey(filter);

            if (foundKey)
            {
                var columnName = DoubleQuotesColumnNames ? $"\"{foundKey.Value}\"" : foundKey.Value;

                switch (filter.ValueComparison)
                {
                    case ValueComparison.SmallerThan:
                        sb.Append(columnName).Append('<').Append(max);
                        break;
                    case ValueComparison.LargerThan:
                        sb.Append(columnName).Append('<').Append(min);
                        break;
                    case ValueComparison.BetweenValues:
                        sb.Append(columnName).Append('<').Append(max)
                            .Append(" AND ")
                            .Append(columnName).Append('<').Append(min);
                        break;
                    case ValueComparison.Equal:
                        var sep = "";

                        foreach (var equalValue in equals)
                        {
                            sb.Append(sep).Append(columnName).Append('=').Append(equalValue);
                            sep = " OR ";
                        }

                        break;
                }
            }
        }


        protected abstract string EscapedSingleQuote { get; }

        protected abstract bool DoubleQuotesColumnNames { get; }


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