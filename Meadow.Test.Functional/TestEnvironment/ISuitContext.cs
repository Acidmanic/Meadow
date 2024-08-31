using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Requests.BuiltIn;
using Meadow.Requests.GenericEventStreamRequests.Models;

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


    List<TModel> Update<TModel>(Func<TModel, bool> predicate, Action<TModel> update) where TModel : class;
    
    List<TModel> Save<TModel>(Func<TModel, bool> predicate, Action<TModel> update, string? collectionName=null) where TModel : class;
    
    TModel? Save<TModel>(TModel model, string? collectionName=null) where TModel : class;
    
    ReadByIdRequest<TModel, TId> ReadById<TModel, TId>(TId id, bool fullTree = false) where TModel : class;
    
    ReadAllRequest<TModel> ReadAll<TModel>(bool fullTree = false) where TModel : class;
    
    DeleteById<TEntity,TId> DeleteById<TEntity,TId>(TId id);
    
    List<StreamEvent> EventStreamRead<TEvent,TEventId,TStreamId>();
    
    List<StreamEvent> EventStreamRead<TEvent,TEventId,TStreamId>(TStreamId streamId);
    
    List<StreamEvent> EventStreamRead<TEvent,TEventId,TStreamId>(TEventId baseEventId,long count);
    
    List<StreamEvent> EventStreamRead<TEvent,TEventId,TStreamId>(TStreamId streamId, TEventId baseEventId,long count);
    

}
