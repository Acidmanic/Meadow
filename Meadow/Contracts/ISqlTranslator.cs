using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Meadow.Configuration;
using Microsoft.Extensions.Logging;

namespace Meadow.Contracts;

public interface ISqlTranslator
{
    

    ILogger Logger { get; set; }
    
    MeadowConfiguration Configuration { get; set; }
    

    string TranslateFilterQueryToDbExpression(FilterQuery filterQuery, ColumnNameTranslation translation);

    string TranslateFieldName(Type ownerEntityType,string headlessAddress, bool fullTree);

    string TranslateSearchTerm(Type entityType, string[] searchTerms);


    string TranslateOrders(Type entityType,  OrderTerm[] orders, bool fullTree);
}