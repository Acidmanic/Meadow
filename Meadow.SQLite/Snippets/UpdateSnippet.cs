using System;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.UpdateProcedure)]
public class UpdateSnippet:ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }

    public string DefinitionParameters => T(t => t.GetProcedureDefinitionParameters());

    public string ProcedureDefinition => T(t => t.SqlTranslator.CreateProcedurePhrase(t.Configurations.RepetitionHandling,t.ProcessedType.NameConvention.UpdateProcedureName));

    public string TableName => T(t=> t.ProcessedType.NameConvention.TableName);

    public string KeyNoneIdParametersSet => T(t => t.GetNoneAutoGeneratedSets());
    
    public string KeyEntityFilterSegment => T(t => t.GetEntityFiltersWhereClause(" AND "," "));

    public string KeyIdFieldName => T(t => t.ProcessedType.IdParameter.Name);
    
    private string T(Func<SnippetToolbox, string> pickValue)
    {
        if (Toolbox is { } toolbox)
        {
            return pickValue(toolbox);
        }

        return string.Empty;
    }
    public string Template => $@"
{ProcedureDefinition} ({DefinitionParameters}) AS

    UPDATE {TableName} SET {KeyNoneIdParametersSet} WHERE {TableName}.{KeyIdFieldName}=@{KeyIdFieldName}{KeyEntityFilterSegment};
    SELECT * FROM {TableName} WHERE {TableName}.{KeyIdFieldName}=@{KeyIdFieldName}{KeyEntityFilterSegment};
GO
".Trim();
}