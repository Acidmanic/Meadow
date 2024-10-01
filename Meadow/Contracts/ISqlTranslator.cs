using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Configuration;
using Meadow.Models;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Contracts;

public partial interface ISqlTranslator
{

    public static readonly ISqlTranslator Null = new NullSqlTranslator();
    

    ILogger Logger { get; set; }
    
    MeadowConfiguration Configuration { get; }
    
    string ColumnNameAliasQuote { get; }

    string AliasTableName(string name);
    
    bool DoubleQuotesColumnNames { get; }

    bool DoubleQuotesTableNames { get; }
    
    bool DoubleQuotesProcedureParameterNames { get; }
    
    
    bool ParameterLessProcedureDefinitionParentheses { get; }
    
    bool UsesSemicolon { get; }
    
    ColumnNameTranslation EntityFilterWhereClauseColumnTranslation { get; }
    
    string TranslateFilterQueryToDbExpression(FilterQuery filterQuery, ColumnNameTranslation translation, string? overrideTableName= null);

    string TranslateFieldName(Type ownerEntityType,string headlessAddress, bool fullTree);

    string TranslateSearchTerm(Type entityType, string[]? searchTerms);

    string TranslateOrders(Type entityType,  OrderTerm[] orders, bool fullTree);

    string CreateProcedurePhrase(RepetitionHandling repetition, string procedureName);
    
    string CreateTablePhrase(RepetitionHandling repetition, string tableName);

    TableParameterDefinition TableColumnDefinition(Parameter parameter);

    string EqualityAssertionOperator(bool isString);
    
    string CreateViewPhrase(RepetitionHandling repetition, string viewName);

    string ParameterPrefix(ParameterUsage usage);

    bool ProcedureParameterNamePrefixBeforeQuoting(ParameterUsage usage); 

    string FormatProcedure(string creationPhrase, string parametersPhrase, string bodyContent, string declarations = "", string returnDataTypeName = "");

    string TranslatePagination(Parameter? offset, Parameter? size);
    
    string TranslateValue(object? value);
}
