using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.CreateTable)]
public class TableSnippet : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }


    public string CreateTablePhrase => Toolbox?.DataAccessServiceResolver.SqlTranslator.CreateTablePhrase(
        Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.TableName) ?? string.Empty;

    public string Template => @"{CreateTablePhrase}";
}