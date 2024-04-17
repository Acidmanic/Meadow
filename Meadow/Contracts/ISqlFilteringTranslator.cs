using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.Contracts;

public interface ISqlFilteringTranslator
{

    public static readonly ISqlFilteringTranslator Null = new NullSqlExpressionTranslator();
     
    ILogger Logger { get; }
    
    MeadowConfiguration Configuration { get; }
    
    string TranslateFilterQueryToDbExpression(FilterQuery filterQuery);

    string TranslateFieldName(Type ownerEntityType,string headlessAddress);

    string TranslateSearchTerm(Type entityType, string[] searchTerms);

    string TranslateOrders(Type entityType,  OrderTerm[] orders);
    
    
    private class NullSqlExpressionTranslator : ISqlFilteringTranslator
    {

        public ILogger Logger { get; set; } = NullLogger.Instance;
        public MeadowConfiguration Configuration { get; set; } = new MeadowConfiguration();
        
        public string TranslateFilterQueryToDbExpression(FilterQuery filterQuery)
        {
            LogError();

            return "";
        }

        public string TranslateFieldName(Type ownerEntityType,string headlessAddress)
        {
            LogError();

            return "";
        }

        public string TranslateSearchTerm(Type entityType, string[] searchTerms)
        {
            LogError();

            return "";
        }

        public string TranslateOrders(Type entityType, OrderTerm[] orders)
        {
            LogError();

            return "";
        }

        private void LogError()
        {
            Logger.LogError("Your DataAccessCore implementation does not provide FilterQuery Translation. " +
                                        "You can modify this behavior by creating a DataAccessCore inherited from the one you already are using," +
                                        "and implementing the ProvideFilterQueryTranslator() method in and returning a valid " +
                                        "implementation of interface IFilterQueryTranslator and use this new data access core " +
                                        "instead of the one you already have by calling MeadowEngine.UseDataAccess(new " +
                                        "CoreProvider<YOUR-DATA-ACCESS-CORE>()).");
        }
    }
}