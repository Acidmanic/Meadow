using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.Translators.Contracts;

public interface ITableTranslator
{

    public string TranslateToTable(ProcessedType type);

}