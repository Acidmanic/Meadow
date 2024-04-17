using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Translators;
using Microsoft.Extensions.Logging;

namespace Meadow.Requests.Context;

public class MeadowExecutionContext
{
    public MeadowExecutionContext(ILogger logger, ISqlLanguageTranslator languageTranslator, ISqlFilteringTranslator filteringTranslator, MeadowConfiguration configuration)
    {
        Logger = logger;
        LanguageTranslator = languageTranslator;
        FilteringTranslator = filteringTranslator;
        Configuration = configuration;
    }

    public ILogger Logger { get; }
    
    public ISqlLanguageTranslator LanguageTranslator { get; }
    
    public ISqlFilteringTranslator FilteringTranslator { get; }
    
    public MeadowConfiguration Configuration { get; }
    
    
}