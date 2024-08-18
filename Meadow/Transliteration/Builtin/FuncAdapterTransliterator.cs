using System;

namespace Meadow.Transliteration.Builtin;

internal class FuncAdapterTransliterator:ITransliterationService
{
    private readonly Func<string, string> _transliterate;

    public FuncAdapterTransliterator(Func<string, string> transliterate)
    {
        _transliterate = transliterate;
    }

    public string Transliterate(string text) => _transliterate(text);
}