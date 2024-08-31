using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.MySql.Snippets;

[CommonSnippet(CommonSnippets.CreateTable)]
public class TableSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;


    public string CreateTablePhrase => Toolbox.SqlTranslator.CreateTablePhrase(
        Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.TableName);

    public string Parameters => string.Join(",\n\t\t",Toolbox.ProcessedType.Parameters.Select(Toolbox.SqlTranslator.TableColumnDefinition).ToList());

    public ISnippet Line => new CommentLineSnippet();
    
    public string Template => @"
{CreateTablePhrase}({Parameters});
{Line}";
}