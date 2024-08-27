using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.CreateTable)]
public class TableSnippet : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }


    public string CreateTablePhrase => Toolbox?.NameOrOverride(nc => nc.TableName) ?? string.Empty;

    public string Parameters => string.Join(",\n\t\t",Toolbox?.ProcessedType.Parameters.Select(Toolbox.SqlTranslator.TableColumnDefinition).ToList() ?? new List<string>());

    public ISnippet Split => new SplitSnippet();
    
    public string Template => @"
{CreateTablePhrase}({Parameters});
{Split}
";
}