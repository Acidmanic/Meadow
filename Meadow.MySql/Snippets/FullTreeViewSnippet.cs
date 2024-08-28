using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;
using Meadow.Sql.Extensions;

namespace Meadow.MySql.Snippets;

[CommonSnippet(CommonSnippets.FullTreeView)]
public class FullTreeViewSnippet : ISnippet
{
    public SnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    public ISnippet Line => new CommentLineSnippet();

    public string CreateViewPhrase => Toolbox.SqlTranslator
        .CreateViewPhrase(Toolbox.Configurations.RepetitionHandling,
            Toolbox.ProcessedType.NameConvention.FullTreeViewName);

    public string FullTreeAliasedParameters =>
        Toolbox.FullTreeTranslation.GetFullTreeAliasedParameters();

    public string TableName => Toolbox.SqlTranslator.GetQuoters()
        .QuoteTableName(Toolbox.ProcessedType.NameConvention.TableName);

    public string KeyInnerJoins => Toolbox.FullTreeTranslation.GetInnerJoins();

    public string KeyWhereClause => Toolbox.GetEntityFiltersWhereClause(" WHERE", "");

    public string Template => @"
{Line}
{CreateViewPhrase} AS 
    SELECT {FullTreeAliasedParameters}
    FROM   {TableName}{KeyInnerJoins}{KeyWhereClause};
{Line}
";
}