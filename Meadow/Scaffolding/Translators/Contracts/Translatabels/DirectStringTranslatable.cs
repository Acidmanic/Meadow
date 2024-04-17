namespace Meadow.Scaffolding.Translators.Contracts.Translatabels;

public class DirectStringTranslatable:ITranslatable
{
    public DirectStringTranslatable(string content)
    {
        Content = content;
    }

    public DirectStringTranslatable()
    {
        
    }
    
    public string Content { get; set; } = string.Empty;
    
    
    
    
    public string Translate(int indent = 0)
    {
        return Content;
    }
    
    
    public static implicit operator string(DirectStringTranslatable t) => t.Translate();
    
    public static implicit operator DirectStringTranslatable(string t) => new DirectStringTranslatable(t);
}