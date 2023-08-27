using Acidmanic.Utilities.Filtering;
using Meadow.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.Contracts;

public interface IFilterQueryTranslator
{
    public class NullFilterQueryTranslator : IFilterQueryTranslator
    {
        private NullFilterQueryTranslator()
        {
            
        }

        private static IFilterQueryTranslator _instance = null;
        
        public static IFilterQueryTranslator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NullFilterQueryTranslator();
                }

                return _instance;
            }
        }
        
        public ILogger Logger { get; set; } = NullLogger.Instance;
        public MeadowConfiguration Configuration { get; set; }

        public string TranslateFilterQueryToWhereClause(FilterQuery filterQuery, bool fullTree)
        {
            Logger.LogError("Your DataAccessCore implementation does not provide FilterQuery Translation. " +
                                "You can modify this behavior by creating a DataAccessCore inherited from the one you already are using," +
                                "and implementing the ProvideFilterQueryTranslator() method in and returning a valid " +
                                "implementation of interface IFilterQueryTranslator and use this new data access core " +
                                "instead of the one you already have by calling MeadowEngine.UseDataAccess(new " +
                                "CoreProvider<YOUR-DATA-ACCESS-CORE>()).");
            return "";
        }
        
        
    }

    ILogger Logger { get; set; }
    
    MeadowConfiguration Configuration { get; set; }

    string TranslateFilterQueryToWhereClause(FilterQuery filterQuery, bool fullTree);
}