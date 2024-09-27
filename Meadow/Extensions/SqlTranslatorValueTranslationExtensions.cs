using System.Collections.Generic;
using System.Linq;
using Meadow.Contracts;

namespace Meadow.Extensions;

public static class SqlTranslatorValueTranslationExtensions
{
    public static List<string> TranslateList(this ISqlTranslator translator, IEnumerable<object> values)
        => values.Select(translator.TranslateValue).ToList();
}