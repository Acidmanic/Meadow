namespace Meadow.Scaffolding.Snippets.Builtin;

public class CommentLineSnippet:ISnippet
{
    public SnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;


    public string Template => @"-- -------------------------------------------------------------------------------------";
}