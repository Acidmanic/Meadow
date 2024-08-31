namespace Meadow.Scaffolding.Snippets.Builtin;

public class TitleBarSnippet:ISnippet
{
    public TitleBarSnippet(string title)
    {
        Toolbox = ISnippetToolbox.Null;
        Title = title;
    }

    public ISnippetToolbox Toolbox { get; set; }

    public string Title { get; }

    public string Template => @"
-- -------------------------------------------------------------------------------------
-- {Title}
-- -------------------------------------------------------------------------------------
".Trim();
}