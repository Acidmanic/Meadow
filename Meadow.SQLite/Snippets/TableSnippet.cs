using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.CreateTable)]
public class TableSnippet : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }


    public string CreateTablePhrase => Toolbox?.SqlTranslator.CreateTablePhrase(
        Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.TableName) ?? string.Empty;

    public List<string> Parameters => Toolbox?.ProcessedType.Parameters.Select(Toolbox.SqlTranslator.TableColumnDefinition).ToList() ?? new List<string>();
    
    public string Template => @"{CreateTablePhrase}({Parameters});";
}