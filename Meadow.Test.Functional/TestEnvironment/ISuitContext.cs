using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Test.Functional.GenericRequests;

namespace Meadow.Test.Functional.TestEnvironment;

public interface ISuitContext
{
    string[] Transliterate(params string[] searchTerms);
    MeadowEngine Engine { get; }
    
    CaseData Data { get; }
    
    public string DatabaseName { get; }

    FindPagedRequest<TModel> FindPaged<TModel>(Action<FilterQueryBuilder<TModel>> filter = null, int offset = 0,
        int size = 1000, Action<OrderSetBuilder<TModel>> order = null,bool fullTree = false, params string[] searchTerms)
        where TModel : class;
    
    void Index<TModel>(IEnumerable<TModel> items);


    List<TModel> Update<TModel>(Func<TModel, bool> predicate, Action<TModel> update) where TModel : class, new();
}
