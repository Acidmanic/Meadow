using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Models;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.Sql
{
    public abstract class SqlTranslatorBase : ISqlTranslator
    {
        
        public ILogger Logger { get; set; }
        
        public MeadowConfiguration Configuration { get; }

        private readonly List<ICast> _externalCasts;

        protected SqlTranslatorBase(MeadowConfiguration configuration)
        {
            _externalCasts = new List<ICast>();
            
            Configuration = configuration;
            
            _externalCasts.AddRange(configuration.ExternalTypeCasts);

            Logger = NullLogger.Instance;

            Configuration = MeadowConfiguration.Null;
        }
        

        public virtual ColumnNameTranslation EntityFilterWhereClauseColumnTranslation => ColumnNameTranslation.DataOwnerDotColumnName;

        public string TranslateFilterQueryToDbExpression(FilterQuery filterQuery, ColumnNameTranslation translation, string? overrideTableName= null)
        {
            var q = this.GetQuoters();

            Func<FilterItem, Result<string>> pickColumName = item => new Result<string>(true, q.QuoteColumnName(item.Key));

            if (translation == ColumnNameTranslation.FullTree)
            {
                var columns = Configuration.GetFullTreeMap(filterQuery.EntityType);

                pickColumName = item => new Result<string>(true, q.QuoteColumnName(columns.GetColumnName(item.Key)));
            }
            else if (translation == ColumnNameTranslation.DataOwnerDotColumnName)
            {
                string tableName;

                if (overrideTableName is { } tName)
                {
                    tableName = tName;
                }
                else
                {
                    tableName = Configuration.TableNameProvider.GetNameForOwnerType(filterQuery.EntityType);
                                
                }

                tableName = q.QuoteTableName(tableName);
                
                pickColumName = item => new Result<string>(true,tableName+ "." + q.QuoteColumnName(item.Key));
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

        public abstract TableParameterDefinition TableColumnDefinition(Parameter parameter);
        public virtual string EqualityAssertionOperator(bool isString) => isString ? "like" : "=";
        

        public virtual string FormatProcedure(string creationPhrase, string parametersPhrase, string bodyContent, string declarations = "", string returnDataTypeName = "")
        {
            if (ParameterLessProcedureDefinitionParentheses || !string.IsNullOrWhiteSpace(parametersPhrase))
            {
                parametersPhrase = $"({parametersPhrase})";
            }

            return creationPhrase + parametersPhrase + "\nAS\n" + bodyContent + "\nGO\n";
        }

        public abstract string TranslatePagination(Parameter offset, Parameter size);


        public abstract string CreateViewPhrase(RepetitionHandling repetition, string viewName);
        
        public virtual string ParameterPrefix(ParameterUsage usage)
        {
            if (usage == ParameterUsage.ProcedureBody) return "@";
            if (usage == ParameterUsage.ProcedureDeclaration) return "@";
            return string.Empty;
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
            var max = TranslateValue(filter.Maximum);
            var min = TranslateValue(filter.Minimum);
            var equalities = TranslateValue(filter.EqualityValues);

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

        
        public string TranslateValue(object? value)
        {
            if (value is { } v)
            {
                if (v is Parameter p)
                {
                    return this.Decorate(p,ParameterUsage.ProcedureBody);
                }

                if (v is Code c)
                {
                    return c.Value;
                }
                var translatingType = v.GetType();

                var translatingObject = v;

                var altered = translatingType.GetAlteredOrOriginal();

                if (altered is { } alteredType && alteredType != translatingType)
                {
                    translatingObject = v.CastTo(alteredType, _externalCasts);

                    translatingType = alteredType;
                }

                return Translate(translatingType, translatingObject);
            }

            return TranslateNull();
        }
        
        protected virtual string TranslateBoolean(bool value)
        {
            if (value)
            {
                return "1";
            }

            return "0";
        }

        private string Translate(Type type, object v)
        {
            if (type == typeof(string))
            {
                var stringValue = (v as string)!;
            
                var escaped = stringValue.Replace($"{StringQuote}", EscapedStringValueQuote);

                return $"{StringQuote}{escaped}{StringQuote}";
            }
        
            if (type == typeof(Guid))
            {
                var stringValue = $"{v}";

                return $"{StringQuote}{stringValue}{StringQuote}";
            }

            if (type == typeof(bool)) return TranslateBoolean((bool)v);

            if (TypeCheck.IsNumerical(type)) return $"{v}";

            return $"{v}";
        }

        protected virtual string TranslateNull() => "null";


        public string AliasTableName(string name) => $"AS {name}";

        public abstract bool DoubleQuotesColumnNames { get; }

        public abstract bool DoubleQuotesTableNames { get; }

        public virtual bool DoubleQuotesProcedureParameterNames => false;

        public virtual bool ProcedureParameterNamePrefixBeforeQuoting(ParameterUsage usage) => false;

        protected virtual string EmptyConditionExpression => "";

        protected virtual string NotEqualOperator => "!=";

        protected virtual string EmptyOrderExpression(Type entityType, bool fullTree) => string.Empty;
        
        public virtual string ColumnNameAliasQuote => "'";
        
        protected virtual char StringQuote => '\'';

        public virtual bool ParameterLessProcedureDefinitionParentheses => false;

        public virtual bool UsesSemicolon => true;
        
        protected abstract string EscapedStringValueQuote { get; }
        
        
    }
}