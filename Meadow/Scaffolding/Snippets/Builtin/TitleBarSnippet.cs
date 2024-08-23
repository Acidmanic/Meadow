namespace Meadow.Scaffolding.Snippets.Builtin;

public class TitleBarSnippet:ISnippet
{
    public TitleBarSnippet(string title)
    {
        Toolbox = null;
        Title = title;
    }

    public SnippetToolbox? Toolbox { get; set; }

    public string Title { get; }

    public string Template => @"
-- -------------------------------------------------------------------------------------
-- {Title}
-- -------------------------------------------------------------------------------------
".Trim();
}