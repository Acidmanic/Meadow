using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.Contracts;

public interface ISqlExpressionTranslator
{
    public class NullSqlExpressionTranslator : ISqlExpressionTranslator
    {
        private NullSqlExpressionTranslator()
        {
            
        }

        private static ISqlExpressionTranslator _instance = null;
        
        public static ISqlExpressionTranslator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NullSqlExpressionTranslator();
                }

                return _instance;
            }
        }
        
        public ILogger Logger { get; set; } = NullLogger.Instance;
        public MeadowConfiguration Configuration { get; set; }
        
        public string TranslateFilterQueryToDbExpression(FilterQuery filterQuery, bool fullTree)
        {
            LogError();

            return "";
        }

        public string TranslateFieldName(Type ownerEntityType,string headlessAddress, bool fullTree)
        {
            LogError();

            return "";
        }

        public string TranslateSearchTerm(Type entityType, string[] searchTerms)
        {
            LogError();

            return "";
        }

        public string TranslateOrders(Type entityType, OrderTerm[] orders,bool fullTree)
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

    ILogger Logger { get; set; }
    
    MeadowConfiguration Configuration { get; set; }
    

    string TranslateFilterQueryToDbExpression(FilterQuery filterQuery, bool fullTree);

    string TranslateFieldName(Type ownerEntityType,string headlessAddress, bool fullTree);

    string TranslateSearchTerm(Type entityType, string[] searchTerms);


    string TranslateOrders(Type entityType,  OrderTerm[] orders, bool fullTree);
}