using Meadow.Scaffolding.Snippets;

namespace Meadow.Test.Functional.Suits.Snippet.Snippets;

public class MethodSnippet:ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = ISnippetToolbox.Null;


    public string Method(string parameter) => $"PARAMS_{parameter}_PARAMS";
    
    public string Method() => "NO_PARAMS";

    public string Template => "SOMETEXT{Method}{/Method}SOMETEXT{Method}POOO{/Method}";
    
    public static string Expected => "SOMETEXTNO_PARAMSSOMETEXTPARAMS_POOO_PARAMS";
}