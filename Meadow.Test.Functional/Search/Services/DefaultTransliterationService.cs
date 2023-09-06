using Meadow.Test.Functional.Search.Contracts;

namespace Meadow.Test.Functional.Search.Services
{
    public class DefaultTransliterationService:ITransliterationService
    {
        public string Transliterate(string text)
        {
            return text;
        }
    }
}