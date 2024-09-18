using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Configuration;
using Meadow.Models;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.Contracts;

public partial interface ISqlTranslator
{
    private class NullSqlTranslator : ISqlTranslator
    {
        public ILogger Logger { get; set; } = NullLogger.Instance;
        public MeadowConfiguration Configuration { get; set; } = new();

        public string AliasQuote => ErrorAndTranslateEmpty();

        public bool DoubleQuotesColumnNames => ErrorAndReturnFalse();
        public bool DoubleQuotesTableNames => ErrorAndReturnFalse();
        public bool DoubleQuotesProcedureParameterNames => ErrorAndReturnFalse();
        public bool ProcedureParameterNamePrefixBeforeQuoting => ErrorAndReturnFalse();

        public bool ParameterLessProcedureDefinitionParentheses => ErrorAndReturnFalse();
        public bool UsesSemicolon => ErrorAndReturnFalse();

        public ColumnNameTranslation EntityFilterWhereClauseColumnTranslation
        {
            get
            {
                LogError();

                return ColumnNameTranslation.ColumnNameOnly;
            }
        }

        public string TranslateFilterQueryToDbExpression(FilterQuery filterQuery, ColumnNameTranslation translation) =>
            ErrorAndTranslateEmpty();

        public string TranslateFieldName(Type ownerEntityType, string headlessAddress, bool fullTree) =>
            ErrorAndTranslateEmpty();

        public string TranslateSearchTerm(Type entityType, string[] searchTerms) => ErrorAndTranslateEmpty();
        public string TranslateOrders(Type entityType, OrderTerm[] orders, bool fullTree) => ErrorAndTranslateEmpty();

        public string CreateProcedurePhrase(RepetitionHandling repetition, string procedureName) =>
            ErrorAndTranslateEmpty();

        public string CreateTablePhrase(RepetitionHandling repetition, string tableName) => ErrorAndTranslateEmpty();
        public TableParameterDefinition TableColumnDefinition(Parameter parameter) => ErrorAndReturnEmptyDefinitions();
        public string CreateViewPhrase(RepetitionHandling repetition, string viewName) => ErrorAndTranslateEmpty();
        public string ProcedureBodyParameterNamePrefix => ErrorAndTranslateEmpty();
        public string ProcedureDefinitionParameterNamePrefix => ErrorAndTranslateEmpty();
        public string FormatProcedure(string creationPhrase, string parametersPhrase, string bodyContent,string declarations = "", string returnDataTypeName="") => ErrorAndTranslateEmpty();
        
        private string ErrorAndTranslateEmpty()
        {
            LogError();

            return string.Empty;
        }

        private bool ErrorAndReturnFalse()
        {
            LogError();

            return false;
        }
        
        private TableParameterDefinition ErrorAndReturnEmptyDefinitions()
        {
            LogError();

            return new TableParameterDefinition(string.Empty,string.Empty);
        }

        private void LogError()
        {
            Logger.LogError("Your DataAccessCore implementation does not provide FilterQuery Translation. " +
                            "You can modify this behavior by creating a DataAccessCore inherited from the one you already are using," +
                            "and implementing the ProvideFilterQueryTranslator() method in and returning a valid " +
                            "implementation of interface IFilterQueryTranslator and use this new data access core " +
                            "instead of the one you already have by calling MeadowEngine.UseDataAccess(new " +
                            "CoreProvider<YOUR-DATA-ACCESS-CORE>()).");
        }
    }
}