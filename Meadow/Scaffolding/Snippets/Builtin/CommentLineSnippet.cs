namespace Meadow.Scaffolding.Snippets.Builtin;

public class CommentLineSnippet:ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;


    public string Template => @"-- -------------------------------------------------------------------------------------";
}