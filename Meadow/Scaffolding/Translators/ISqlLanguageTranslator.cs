using Meadow.Scaffolding.Translators.Contracts;

namespace Meadow.Scaffolding.Translators;

public interface ISqlLanguageTranslator:
    IMeadowConfigurationTranslator,
    IExpressionTranslator,
    ISyntaxTranslator,
    ITableTranslator
{
   


}