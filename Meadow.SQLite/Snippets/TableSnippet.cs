using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.CreateTable)]
public class TableSnippet : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }


    public string CreateTablePhrase => Toolbox?.SqlTranslator.CreateTablePhrase(
        Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.TableName) ?? string.Empty;

    public string Parameters => string.Join(",\n\t\t",Toolbox?.ProcessedType.Parameters.Select(Toolbox.SqlTranslator.TableColumnDefinition).ToList() ?? new List<string>());

    public ISnippet Line => new CommentLineSnippet();
    
    public string Template => @"
{CreateTablePhrase}({Parameters});
{Line}
-- SPLIT
{Line}
";
}