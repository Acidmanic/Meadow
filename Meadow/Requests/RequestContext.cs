using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessResolving;
using Meadow.DataTypeMapping;
using Meadow.Transliteration;

namespace Meadow.Requests;

public class RequestContext
{
    public RequestContext(MeadowConfiguration configuration)
    {
        var dataAccessServiceResolver = new DataAccessServiceResolver(configuration);
        
        Configuration = configuration;

        Translator = dataAccessServiceResolver.SqlTranslator;
        Transliterator = configuration.TransliterationService;
        ValueTranslator = dataAccessServiceResolver.ValueTranslator;
        TypeNameMapper = dataAccessServiceResolver.DbTypeNameMapper;
    }

    public ISqlTranslator Translator { get; }
    
    public ITransliterationService Transliterator { get; }
    
    public IValueTranslator ValueTranslator { get; }
    
    public IDbTypeNameMapper TypeNameMapper { get; }
    
    public MeadowConfiguration Configuration { get; }

    public IndexCorpusService<T> GetCorpusService<T>() => new IndexCorpusService<T>(Configuration.TransliterationService);

}