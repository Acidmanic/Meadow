using System;
using System.Linq;
using Meadow.Contracts;
using Meadow.Enums;
using Meadow.Models;
using Meadow.Scaffolding.Models;

namespace Meadow.Extensions;

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
    
    public static string QuoteTable(this ISqlTranslator tr, string name)
    {
        if (tr.DoubleQuotesTableNames) return "\""+name+"\"";
        
        return name;
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


    public static string AliasColumnName(this ISqlTranslator tr, string columnName)
    {
        return $" {tr.ColumnNameAliasQuote}{columnName}{tr.ColumnNameAliasQuote}";
    }

    private static string AliasOrDirect(ISqlTranslator tr,string name, string? alias)
    {
        if (alias is { } a) return name +  AliasColumnName(tr, a);

        return name;
    }
    
    private static string TranslateSelectField(this ISqlTranslator tr, SelectField field)
    {
        if (field.Type == SelectFieldType.All) return "*";

        if (field.Type == SelectFieldType.Code) return AliasOrDirect(tr, field.Code, field.Alias);

        if (field.Type == SelectFieldType.ColumnName)
        {
            var column = GetQuoters(tr).QuoteColumnName(field.Code);

            return AliasOrDirect(tr, column, field.Alias);
        }

        if (field.Type == SelectFieldType.ProcedureParameter)
        {
            var parameter = tr.Decorate(field.Code, ParameterUsage.ProcedureBody);

            return AliasOrDirect(tr, parameter, field.Alias);
        }

        return "*";
    }

    public static string TranslateSelectFields(this ISqlTranslator tr, params SelectField[] fields)
        => string.Join(',', fields.Select(f => TranslateSelectField(tr, f)));

}