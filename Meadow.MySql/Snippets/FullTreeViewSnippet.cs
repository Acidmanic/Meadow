using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;
using Meadow.Sql.Extensions;

namespace Meadow.MySql.Snippets;

[CommonSnippet(CommonSnippets.FullTreeView)]
public class FullTreeViewSnippet : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }

    public ISnippet Line => new CommentLineSnippet();

    public string CreateViewPhrase => Toolbox?.SqlTranslator
        .CreateViewPhrase(Toolbox.Configurations.RepetitionHandling,
            Toolbox.ProcessedType.NameConvention.FullTreeViewName) ?? string.Empty;

    public string FullTreeAliasedParameters =>
        Toolbox?.FullTreeTranslation.GetFullTreeAliasedParameters() ?? string.Empty;

    public string TableName => Toolbox?.SqlTranslator.GetQuoters()
        .QuoteTableName(Toolbox.ProcessedType.NameConvention.TableName) ?? string.Empty;

    public string KeyInnerJoins => Toolbox?.FullTreeTranslation.GetInnerJoins() ?? string.Empty;

    public string KeyWhereClause => Toolbox?.GetEntityFiltersWhereClause(" WHERE", "") ?? string.Empty;

    public string Template => @"
{Line}
{CreateViewPhrase} AS 
    SELECT {FullTreeAliasedParameters}
    FROM   {TableName}{KeyInnerJoins}{KeyWhereClause};
{Line}
";
}