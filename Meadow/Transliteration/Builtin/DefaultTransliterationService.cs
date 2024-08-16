namespace Meadow.Transliteration.Builtin
{
    public class DefaultTransliterationService:ITransliterationService
    {
        public string Transliterate(string text)
        {
            return text;
        }
    }
}