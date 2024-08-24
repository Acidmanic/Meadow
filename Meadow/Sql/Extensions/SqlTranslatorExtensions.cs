using System;
using Meadow.Contracts;

namespace Meadow.Sql.Extensions;


public record QuoterSet(Func<string, string> QuoteTableName, Func<string, string> QuoteColumnName);

public static class SqlTranslatorExtensions
{
    
    public static QuoterSet GetQuoters(this ISqlTranslator tr) => new QuoterSet(
        tr.DoubleQuotesTableNames ? s => $"\"{s}\"" : s => s,
        tr.DoubleQuotesColumnNames ? s => $"\"{s}\"" : s => s);
    
}