using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Transliteration;

namespace Meadow.Requests;

public class RequestContext
{
    public RequestContext(ISqlExpressionTranslator translator, MeadowConfiguration configuration)
    {
        Translator = translator;
        
        Configuration = configuration;
    }

    public ISqlExpressionTranslator Translator { get; }


    public ITransliterationService Transliterator => Configuration.TransliterationService;
    public MeadowConfiguration Configuration { get; }

    public IndexCorpusService<T> GetCorpusService<T>() => new IndexCorpusService<T>(Configuration.TransliterationService);

}