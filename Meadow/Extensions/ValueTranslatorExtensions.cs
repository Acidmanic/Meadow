using System.Collections.Generic;
using System.Linq;
using Meadow.DataTypeMapping;

namespace Meadow.Extensions;

public static class ValueTranslatorExtensions
{
    public static List<string> TranslateList(this IValueTranslator translator, IEnumerable<object> values)
        => values.Select(translator.Translate).ToList();
}