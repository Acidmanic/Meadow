using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Meadow.Contracts;

public class NullSqlTranslator : ISqlTranslator
{
    private NullSqlTranslator()
    {
    }

    private static ISqlTranslator? _instance;

    public static ISqlTranslator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NullSqlTranslator();
            }

            return _instance;
        }
    }

    public ILogger Logger { get; set; } = NullLogger.Instance;
    public MeadowConfiguration Configuration { get; set; } = new();

    public string TranslateFilterQueryToDbExpression(FilterQuery filterQuery, ColumnNameTranslation translation)
    {
        LogError();

        return "";
    }

    public string TranslateFieldName(Type ownerEntityType, string headlessAddress, bool fullTree)
    {
        LogError();

        return "";
    }

    public string TranslateSearchTerm(Type entityType, string[] searchTerms)
    {
        LogError();

        return "";
    }

    public string TranslateOrders(Type entityType, OrderTerm[] orders, bool fullTree)
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