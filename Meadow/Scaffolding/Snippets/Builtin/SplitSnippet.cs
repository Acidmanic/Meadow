namespace Meadow.Scaffolding.Snippets.Builtin;

public class SplitSnippet:ISnippet
{
    public SplitSnippet()
    {
        Toolbox = SnippetToolbox.Null;
    }

    public ISnippetToolbox Toolbox { get; set; } 
    

    public string Template => @"
-- -------------------------------------------------------------------------------------
-- SPLIT
-- -------------------------------------------------------------------------------------
".Trim();
}