namespace Meadow.Scaffolding.Snippets.Builtin;

public class StringSnippet:ISnippet
{
    public StringSnippet(string value)
    {
        Template = value;
    }

    public ISnippetToolbox Toolbox { get; set; } = ISnippetToolbox.Null;

    public string Template { get; }
}