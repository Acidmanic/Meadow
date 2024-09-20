using System;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Models;
using Meadow.Scaffolding.Models;

namespace Meadow.Sql.Extensions;

public record QuoterSet(Func<string, string> QuoteTableName, Func<string, string> QuoteColumnName,
    Func<string, string> QuoteParameterDeclarationName);

public record ParameterDecoratorSet(
    Func<string, string> AsColumnName,
    Func<string, string> AsParameterDeclaration,
    Func<string, string> AsProcedureBodyParameter);

public static class SqlTranslatorExtensions
{
    public static QuoterSet GetQuoters(this ISqlTranslator tr) => new QuoterSet(
        tr.DoubleQuotesTableNames ? s => $"\"{s}\"" : s => s,
        tr.DoubleQuotesColumnNames ? s => $"\"{s}\"" : s => s,
        tr.DoubleQuotesProcedureParameterNames ? s => $"\"{s}\"" : s => s);

    public static ParameterDecoratorSet GetParameterDecorators(this ISqlTranslator tr)
    {
        return new ParameterDecoratorSet(
            GetParameterDecorator(tr, ParameterUsage.ColumnName),
            GetParameterDecorator(tr, ParameterUsage.ProcedureDeclaration),
            GetParameterDecorator(tr, ParameterUsage.ProcedureBody));
    }

    public static bool Quotes(this ISqlTranslator tr, ParameterUsage usage)
    {
        if (usage == ParameterUsage.ColumnName) return tr.DoubleQuotesColumnNames;

        return tr.DoubleQuotesProcedureParameterNames;
    }

    public static Func<string, string> GetParameterDecorator(this ISqlTranslator tr, ParameterUsage usage)
    {
        Func<string, string> decorator = s => tr.ParameterPrefix(usage) + s;

        if (Quotes(tr, usage))
        {
            if (tr.ProcedureParameterNamePrefixBeforeQuoting(usage))
            {
                decorator = s => $"\"{tr.ParameterPrefix(usage) + s}\"";
            }
            else
            {
                decorator = s => tr.ParameterPrefix(usage) + $"\"{s}\"";
            }
        }

        return decorator;
    }

    public static string Decorate(this ISqlTranslator tr, Parameter parameter, ParameterUsage usage)
        => Decorate(tr, parameter.Name, usage);

    public static string Decorate(this ISqlTranslator tr, string parameterName, ParameterUsage usage)
        => GetParameterDecorator(tr, usage)(parameterName);

    public static string EqualityAssertionOperator(this ISqlTranslator sqlTranslator, Parameter p)
        => sqlTranslator.EqualityAssertionOperator(p.IsString);
    
}