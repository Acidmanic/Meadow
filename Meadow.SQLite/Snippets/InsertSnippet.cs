using System;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.InsertProcedure)]
public class InsertSnippet:ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }

    public string KeyHeaderCreation => T(t => t.SqlTranslator.CreateProcedurePhrase(t.Configurations.RepetitionHandling,t.ProcessedType.NameConvention.InsertProcedureName));
    public string KeyParameters => T(t => t.GetProcedureDefinitionParameters());
    public string KeyTableName => T(t => t.ProcessedType.NameConvention.TableName);
    public string KeyInsertColumns => T(t => t.GetInsertColumns());
    public string KeyInsertValues => T(t => t.GetInsertValues());
    public string KeyEntityFilterSegment => T(t => t.GetEntityFiltersWhereClause(" AND "," "));
    
    
    
    private string T(Func<SnippetToolbox, string> pickValue)
    {
        if (Toolbox is { } toolbox)
        {
            return pickValue(toolbox);
        }

        return string.Empty;
    }
    public string Template => $@"
{KeyHeaderCreation}({KeyParameters}) AS
    INSERT INTO {KeyTableName} ({KeyInsertColumns})
    VALUES ({KeyInsertValues});
    SELECT * FROM {KeyTableName} WHERE ROWID=LAST_INSERT_ROWID(){KeyEntityFilterSegment};
GO
".Trim();
}