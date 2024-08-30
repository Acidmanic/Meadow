using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Requests.BuiltIn;
using Meadow.Requests.GenericEventStreamRequests.Models;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.TestEnvironment;

public interface ISuitContext
{
    MeadowEngine Engine { get; }
    
    CaseData Data { get; }
    
    public string DatabaseName { get; }
    
    MeadowConfiguration MeadowConfiguration { get; }
    

    FindPagedRequest<TModel> FindPaged<TModel>(Action<FilterQueryBuilder<TModel>>? filter = null, int offset = 0,
        int size = 1000, Action<OrderSetBuilder<TModel>>? order = null,bool fullTree = false, params string[] searchTerms)
        where TModel : class;
    
    void Index<TModel>(IEnumerable<TModel> items);


    List<TModel> Update<TModel>(Func<TModel, bool> predicate, Action<TModel> update) where TModel : class, new();
    
    List<TModel> Save<TModel>(Func<TModel, bool> predicate, Action<TModel> update, string? collectionName=null) where TModel : class, new();
    
    TModel? Save<TModel>(TModel model, string? collectionName=null) where TModel : class, new();
    
    ReadByIdRequest<TModel, TId> ReadById<TModel, TId>(TId id, bool fullTree = false) where TModel : class, new();
    
    ReadAllRequest<TModel> ReadAll<TModel>(bool fullTree = false) where TModel : class, new();
    
    DeleteById<TEntity,TId> DeleteById<TEntity,TId>(TId id);
    
    List<ObjectEntry<TEventId,TStreamId>> EventStreamRead<TEvent,TEventId,TStreamId>();

}
