using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.MySql.Snippets;

[CommonSnippet(CommonSnippets.CreateTable)]
public class TableSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    public ISnippet Split => new SplitSnippet();

    public string Table => Toolbox.TranslateTable( Toolbox.ProcessedType.Parameters);
    
    public string Template => "{Table}\n{Split}";
}