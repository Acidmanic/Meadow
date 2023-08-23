using System;
using System.Collections.Generic;
using System.Text;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection;
using Meadow.Contracts;
using Microsoft.Extensions.Logging;

namespace Meadow.SqlServer
{
    public class SqlServerFilterQueryTranslator : IFilterQueryTranslator
    {
        public ILogger Logger { get; set; }

        public string TranslateFilterQueryToWhereClause(FilterQuery filterQuery)
        {
            var sb = new StringBuilder();

            var anyFilters = false;

            var sep = "";

            foreach (var filter in filterQuery.Items())
            {
                anyFilters = true;


                sb.Append(sep).Append("(");

                Append(sb, filter);

                sb.Append(")");

                sep = " AND ";
            }

            if (anyFilters)
            {
                return " WHERE " + sb.ToString();
            }

            return "";
        }

        private void Append(StringBuilder sb, FilterItem filter)
        {
            var max = HandleQuotingAndEscaping(filter.Maximum, filter.ValueType);
            var min = HandleQuotingAndEscaping(filter.Minimum, filter.ValueType);
            var equals = HandleQuotingAndEscaping(filter.EqualValues, filter.ValueType);

            switch (filter.ValueComparison)
            {
                case ValueComparison.SmallerThan:
                    sb.Append(filter.Key).Append('<').Append(max);
                    break;
                case ValueComparison.LargerThan:
                    sb.Append(filter.Key).Append('<').Append(min);
                    break;
                case ValueComparison.BetweenValues:
                    sb.Append(filter.Key).Append('<').Append(max)
                        .Append(" AND ")
                        .Append(filter.Key).Append('<').Append(min);
                    break;
                case ValueComparison.Equal:
                    var sep = "";

                    foreach (var equalValue in equals)
                    {
                        sb.Append(sep).Append(filter.Key).Append('=').Append(equalValue);
                        sep = " OR ";
                    }

                    break;
            }
        }

        private string HandleQuotingAndEscaping(string value, Type type)
        {
            if (value == null)
            {
                return DefaultValue(type);
            }

            if (type == typeof(string))
            {
                var escaped = value.Replace("'", "''");

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