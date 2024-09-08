using System;
using System.Linq;
using System.Text;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Results;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Sql.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.Sql
{
    public abstract class SqlTranslatorBase : ISqlTranslator
    {

        private readonly IValueTranslator _valueTranslator;
        
        public ILogger Logger { get; set; }
        public MeadowConfiguration Configuration { get; set; }
        
        
        protected SqlTranslatorBase(IValueTranslator valueTranslator)
        {
            _valueTranslator = valueTranslator;

            Logger = NullLogger.Instance;
            
            Configuration = MeadowConfiguration.Null;
        }
        

        public virtual string AliasQuote => "'";

        public virtual ColumnNameTranslation EntityFilterWhereClauseColumnTranslation =>
            ColumnNameTranslation.DataOwnerDotColumnName;

        public string TranslateFilterQueryToDbExpression(FilterQuery filterQuery, ColumnNameTranslation translation)
        {
            var q = this.GetQuoters();
            
            Func<FilterItem, Result<string>> pickColumName = item => new Result<string>(true, q.QuoteColumnName(item.Key));

            if (translation == ColumnNameTranslation.FullTree)
            {
                var columns = Configuration.GetFullTreeMap(filterQuery.EntityType);

                pickColumName = item => new Result<string>(true,q.QuoteColumnName(columns.GetColumnName(item.Key)));
            }
            else if (translation == ColumnNameTranslation.DataOwnerDotColumnName)
            {
                pickColumName = item => new Result<string>(true,
                    q.QuoteTableName(Configuration.TableNameProvider.GetNameForOwnerType(filterQuery.EntityType))
                    + "." + q.QuoteColumnName(item.Key));
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

        public string TranslateSearchTerm(Type entityType, string[]? searchTerms)
        {
            if (searchTerms == null || searchTerms.Length == 0)
            {
                return EmptyConditionExpression;
            }

            var nc = Configuration.GetNameConvention(entityType);

            var q = this.GetQuoters();

            var searchIndexTable = nc.SearchIndexTableName;

            var columnFullName = q.QuoteTableName(searchIndexTable) + "." + q.QuoteColumnName("IndexCorpus");

            return string.Join(" OR ", searchTerms.Select(
                s => $"{columnFullName} like '%{s}%'"));
        }

        public string TranslateOrders(Type entityType, OrderTerm[]? orders, bool fullTree)
        {
            if (orders == null || orders.Length == 0)
            {
                return EmptyOrderExpression(entityType, fullTree);
            }

            var q = this.GetQuoters().QuoteColumnName;
            
            var map = Configuration.GetFullTreeMap(entityType);
            Func<OrderTerm, string> sort = o => o.Sort == OrderSort.Descending ? "DESC" : "ASC";
            Func<OrderTerm, string> column = o => q(TranslateFieldName(map, o.Key, fullTree));

            var terms = orders.Select(o => column(o) + " " + sort(o));

            return string.Join(", ", terms);
        }

        public abstract string CreateProcedurePhrase(RepetitionHandling repetition, string procedureName);

        public abstract string CreateTablePhrase(RepetitionHandling repetition, string tableName);
        
        public abstract string TableColumnDefinition(Parameter parameter);

        public virtual string ProcedureBodyParameterNamePrefix => "@";
        public virtual string ProcedureDefinitionParameterNamePrefix => "@";

        public abstract string CreateViewPhrase(RepetitionHandling repetition, string viewName);

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
            var max = _valueTranslator.Translate(filter.Maximum);
            var min = _valueTranslator.Translate(filter.Minimum);
            var equalities = _valueTranslator.TranslateList(filter.EqualityValues);

            var foundKey = pickKey(filter);

            if (foundKey)
            {
                
                var columnName = foundKey.Value;
                
                string sep;
                
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


        

        public abstract bool DoubleQuotesColumnNames { get; }

        public abstract bool DoubleQuotesTableNames { get; }

        protected virtual string EmptyConditionExpression => "";

        protected virtual string NotEqualOperator => "!=";

        protected virtual string EmptyOrderExpression(Type entityType, bool fullTree)
        {
            return "";
        }

    }
}