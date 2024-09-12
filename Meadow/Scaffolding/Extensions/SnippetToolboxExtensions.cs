using System;
using System.Linq;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Scaffolding.Snippets;

namespace Meadow.Scaffolding.Extensions;

public static class SnippetToolboxExtensions
{
    public static string EventIdDefinitionPhrase(this ISnippetToolbox toolbox, string? tailing = null)
    {
        var postFix = tailing ?? string.Empty;

        var eventIdParameter = EventIdParameter(toolbox);

        return toolbox.SqlTranslator.TableColumnDefinition(eventIdParameter) + postFix;
    }


    public static string GetSelectColumns(this ISnippetToolbox toolbox, ColumnNameTranslation columnNameTranslation = ColumnNameTranslation.ColumnNameOnly)
    {
        if (columnNameTranslation == ColumnNameTranslation.FullTree)
        {
            return string.Join(',', toolbox.ProcessedType.ParametersFullTree.Select(p => p.Name));
        }

        if (columnNameTranslation == ColumnNameTranslation.DataOwnerDotColumnName)
        {
            var effectiveTableName = NameOrOverride(toolbox, nc => nc.TableName);

            return string.Join(',', toolbox.ProcessedType.Parameters.Select(p => $"{effectiveTableName}.{p.Name}"));
        }

        return string.Join(',', toolbox.ProcessedType.Parameters.Select(p => p.Name));
    }

    public static string GetNoneAutoGeneratedColumns(this ISnippetToolbox toolbox)
    {
        return string.Join(',', toolbox.ProcessedType.Parameters
            .Where(p => !p.IdentifierStatus.Is(ParameterIdentifierStatus.AutoGenerated))
            .Select(p => p.Name));
    }


    public static string GetNoneAutoGeneratedValues(this ISnippetToolbox toolbox)
    {
        return string.Join(',', toolbox.ProcessedType.Parameters
            .Where(p => !p.IdentifierStatus.Is(ParameterIdentifierStatus.AutoGenerated))
            .Select(p => toolbox.SqlTranslator.ProcedureBodyParameterNamePrefix + p.Name));
    }

    public static string GetNoneAutoGeneratedSets(this ISnippetToolbox toolbox)
    {
        var noneAutoGeneratedParameters = toolbox.ProcessedType.Parameters
            .Where(p => !p.IdentifierStatus.Is(ParameterIdentifierStatus.AutoGenerated));

        return toolbox.ParameterNameValueSetJoint(noneAutoGeneratedParameters, ",", toolbox.SqlTranslator.ProcedureBodyParameterNamePrefix);
    }

    public static string GetProcedureDefinitionParameters(this ISnippetToolbox toolbox, Func<Parameter, bool>? select = null)
    {
        Func<Parameter, bool> parameterSelector = select ?? (_ => true);

        return string.Join(',', toolbox.ProcessedType.Parameters
            .Where(parameterSelector)
            .Select(p => toolbox.ParameterNameTypeJoint(p, toolbox.SqlTranslator.ProcedureDefinitionParameterNamePrefix)));
    }

    public static bool ActsById(this ISnippetToolbox toolbox) => toolbox.Configurations.IdAwarenessBehavior.Is(IdAwarenessBehavior.UseById);
    public static bool ActsAll(this ISnippetToolbox toolbox) => toolbox.Configurations.IdAwarenessBehavior.Is(IdAwarenessBehavior.UseAll);

    public static string IfById(this ISnippetToolbox toolbox, string byIdTerm, string notByItTerm = "") => ActsById(toolbox) ? byIdTerm : notByItTerm;


    public static string GetIdAwareProcedureDefinitionParametersPhrase(this ISnippetToolbox toolbox) =>
        GetIdAwareProcedureDefinitionParametersPhrase(toolbox, ActsById(toolbox));


    public static Parameter[] GetIdAwareProcedureDefinitionParameters(this ISnippetToolbox toolbox, bool byId)
    {
        if (byId && toolbox.ProcessedType.HasId)
        {
            return new[] { toolbox.ProcessedType.IdParameter };
        }

        return new Parameter[] { };
    }
    
    public static string GetIdAwareProcedureDefinitionParametersPhrase(this ISnippetToolbox toolbox, bool byId)
    {
        if (byId && toolbox.ProcessedType.HasId)
        {
            return $"({toolbox.ParameterNameTypeJoint(toolbox.ProcessedType.IdParameter, toolbox.SqlTranslator.ProcedureDefinitionParameterNamePrefix)})";
        }

        if (toolbox.SqlTranslator.ParameterLessProcedureDefinitionParentheses)
        {
            return "()";
        }
        return string.Empty;
    }


    public static string WhereByIdClause(this ISnippetToolbox toolbox, bool fullTree = false) => WhereByIdClause(toolbox, ActsById(toolbox), fullTree);

    public static string WhereByIdClause(this ISnippetToolbox toolbox, bool actById, bool fullTree)
    {
        var tableIdParameter = fullTree ? toolbox.ProcessedType.IdParameterFullTree : toolbox.ProcessedType.IdParameter;
        var procedureIdParameter = toolbox.ProcessedType.IdParameter;

        return actById ? $" WHERE {tableIdParameter.Name} = {toolbox.SqlTranslator.ProcedureBodyParameterNamePrefix}{procedureIdParameter?.Name}" : string.Empty;
    }

    public static string TableOrFullViewName(this ISnippetToolbox toolbox, bool fullTreeView = false)
    {
        return fullTreeView ? toolbox.ProcessedType.NameConvention.FullTreeViewName : toolbox.ProcessedType.NameConvention.TableName;
    }

    public static string IdFieldNameOrDefault(this ISnippetToolbox toolbox, string defaultName)
    {
        return toolbox.ProcessedType.HasId ? toolbox.ProcessedType.IdParameter.Name : defaultName;
    }

    public static string IdFieldNameOrDefaultFullTree(this ISnippetToolbox toolbox, string defaultName)
    {
        return toolbox.ProcessedType.HasId ? toolbox.ProcessedType.IdParameterFullTree.Name : defaultName;
    }

    public static string EventIdProcedureParameterPhrase(this ISnippetToolbox toolbox, string? tailing = null)
    {
        var postFix = tailing ?? string.Empty;

        var eventIdParameter = EventIdParameter(toolbox);

        return toolbox.SqlTranslator.ProcedureDefinitionParameterNamePrefix + eventIdParameter.Name + " " + eventIdParameter.Type + postFix;
    }

    public static string CreateReadProcedurePhrase(this ISnippetToolbox toolbox, bool fullTree, bool byId)
    {
        var procedureName = GetReadProcedureName(toolbox, fullTree, byId);

        return toolbox.SqlTranslator.CreateProcedurePhrase(toolbox.Configurations.RepetitionHandling, procedureName);
    }


    public static string GetReadProcedureName(this ISnippetToolbox toolbox, bool fullTree, bool byId)
    {
        if (fullTree)
        {
            return byId ? toolbox.ProcessedType.NameConvention.ReadByIdProcedureNameFullTree : toolbox.ProcessedType.NameConvention.ReadAllProcedureNameFullTree;
        }

        return byId ? toolbox.ProcessedType.NameConvention.ReadByIdProcedureName : toolbox.ProcessedType.NameConvention.ReadAllProcedureName;
    }

    private static Parameter EventIdParameter(this ISnippetToolbox toolbox)
    {
        var eventIdParameter = new Parameter()
        {
            Name = "EventId",
            IdentifierStatus = ParameterIdentifierStatus.Unique,
            IsNumerical = toolbox.ProcessedType.IsEventIdAutogenerated,
            StandardAddress = "EventModel.EventId",
            Type = toolbox.ProcessedType.EventIdTypeName
        };

        return eventIdParameter;
    }

    public static string NameOrOverride(this ISnippetToolbox toolbox, Func<NameConvention, string> pick)
    {
        if (toolbox.Configurations.OverrideDbObjectName)
        {
            return toolbox.Configurations.OverrideDbObjectName.Value(toolbox.Construction);
        }

        return pick(toolbox.ProcessedType.NameConvention);
    }

    public static string CreateTablePhrase(this ISnippetToolbox toolbox, string tableName)
    {
        return toolbox.SqlTranslator.CreateTablePhrase(toolbox.Configurations.RepetitionHandling, tableName);
    }

    public static string CreateTablePhrase(this ISnippetToolbox toolbox)
    {
        return CreateTablePhrase(toolbox, NameOrOverride(toolbox, nc => nc.TableName));
    }


    public static string Procedure(this ISnippetToolbox toolbox, RepetitionHandling repetition, string procedureName, string body, string declarations = "", string returnTypeName = "", params Parameter[] parameters)
    {
        var creationPhrase = toolbox.SqlTranslator.CreateProcedurePhrase(repetition, procedureName);

        var parametersDefinition = toolbox.ParameterNameTypeJoint(parameters, ",", toolbox.SqlTranslator.ProcedureDefinitionParameterNamePrefix);

        return toolbox.SqlTranslator.FormatProcedure(creationPhrase, parametersDefinition, body, declarations, returnTypeName);
    }
}