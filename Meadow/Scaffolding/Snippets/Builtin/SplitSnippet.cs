namespace Meadow.Scaffolding.Snippets.Builtin;

public class SplitSnippet:ISnippet
{
    public SplitSnippet()
    {
        Toolbox = null;
    }

    public SnippetToolbox? Toolbox { get; set; }
    

    public string Template => @"
-- -------------------------------------------------------------------------------------
-- SPLIT
-- -------------------------------------------------------------------------------------
".Trim();
}