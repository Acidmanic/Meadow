using System;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.InsertProcedure)]
public class ReadSnippet:ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }

    public string KeyHeaderCreationShallow => T(t => t.SqlTranslator
        .CreateProcedurePhrase(t.Configurations.RepetitionHandling,t.ProcessedType.NameConvention.ReadAllProcedureName));
    public string KeyHeaderCreationFullTree => T(t => t.SqlTranslator
        .CreateProcedurePhrase(t.Configurations.RepetitionHandling,t.ProcessedType.NameConvention.ReadAllProcedureNameFullTree));
    public string KeyParametersDeclaration => T(t => t.GetReadProcedureDefinitionParametersPhrase());
    public string KeyTableName => T(t => t.ProcessedType.NameConvention.TableName);
    public string KeyWhereClause => T(t => t.WhereByIdClause());
    public string KeyInsertValues => T(t => t.GetInsertValues());
    public string KeyEntityFilterSegment => T(t => t.GetEntityFiltersWhereClause(" AND "," "));

    public ISnippet Line => new CommentLineSnippet();
    
    private string T(Func<SnippetToolbox, string> pickValue)
    {
        if (Toolbox is { } toolbox)
        {
            return pickValue(toolbox);
        }

        return string.Empty;
    }
    public string Template => @"
{KeyHeaderCreationShallow}{KeyParametersDeclaration} AS
    SELECT * FROM {KeyTableName}{KeyWhereClause}
GO
{Line}
{KeyHeaderCreationFullTree}{KeyParametersDeclaration} AS
    SELECT * FROM {KeyTableName}{KeyWhereClause}
GO
".Trim();
}