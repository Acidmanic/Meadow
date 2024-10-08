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

        SqlTranslator = dataAccessServiceResolver.SqlTranslator;
        Transliterator = configuration.TransliterationService;
        TypeNameMapper = dataAccessServiceResolver.DbTypeNameMapper;
    }

    public ISqlTranslator SqlTranslator { get; }
    
    public ITransliterationService Transliterator { get; }
    
    public IDbTypeNameMapper TypeNameMapper { get; }
    
    public MeadowConfiguration Configuration { get; }

    public IndexCorpusService<T> GetCorpusService<T>() => new IndexCorpusService<T>(Configuration.TransliterationService);

}