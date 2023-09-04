using Meadow.Search.Contracts;

namespace Meadow.Search.Services;

public class DefaultTransliterationService:ITransliterationService
{
    public string Transliterate(string text)
    {
        return text;
    }
}