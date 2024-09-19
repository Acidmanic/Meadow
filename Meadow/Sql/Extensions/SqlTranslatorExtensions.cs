using System;
using Meadow.Contracts;
using Meadow.Scaffolding.Models;

namespace Meadow.Sql.Extensions;


public record QuoterSet(Func<string, string> QuoteTableName, Func<string, string> QuoteColumnName, Func<string,string> QuoteParameterDefinitionName);

public static class SqlTranslatorExtensions
{
    
    public static QuoterSet GetQuoters(this ISqlTranslator tr) => new QuoterSet(
        tr.DoubleQuotesTableNames ? s => $"\"{s}\"" : s => s,
        tr.DoubleQuotesColumnNames ? s => $"\"{s}\"" : s => s,
        tr.DoubleQuotesProcedureParameterNames ? s => $"\"{s}\"" : s => s);
    
    
    public static string EqualityAssertionOperator(this ISqlTranslator sqlTranslator, Parameter p) 
        => sqlTranslator.EqualityAssertionOperator(p.IsString);
    
}