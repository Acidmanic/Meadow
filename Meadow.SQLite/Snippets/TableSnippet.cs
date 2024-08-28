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
    public SnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;


    public string CreateTablePhrase => Toolbox.CreateTablePhrase();

    public string Parameters => string.Join(",\n\t\t",Toolbox?.ProcessedType.Parameters.Select(Toolbox.SqlTranslator.TableColumnDefinition).ToList() ?? new List<string>());

    public ISnippet Split => new SplitSnippet();
    
    public string Template => @"
{CreateTablePhrase}({Parameters});
{Split}
";
}