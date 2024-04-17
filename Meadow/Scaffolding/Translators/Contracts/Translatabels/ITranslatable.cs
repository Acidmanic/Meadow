namespace Meadow.Scaffolding.Translators.Contracts.Translatabels;

public static class Translatable
{
    private class EmptyTranslatable : ITranslatable
    {
        public string Translate(int indent=0)
        {
            return string.Empty;
        }
    }

    public static readonly ITranslatable Empty = new EmptyTranslatable();


}

public interface ITranslatable
{
    string Translate(int indent=0);
    
}
