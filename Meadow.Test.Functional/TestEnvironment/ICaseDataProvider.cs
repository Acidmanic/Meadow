using System.Collections.Generic;
using Meadow.Extensions;
using Meadow.Requests.GenericEventStreamRequests.Models;

namespace Meadow.Test.Functional.TestEnvironment;

public interface ICaseDataProvider
{
    void Initialize();

    void PostSeeding();

    List<List<object>> SeedSet { get; }
}

public static class CaseDataProviderExtensions
{


    public static StreamEvent ToStreamEvent<TStreamId>(this object e, TStreamId streamId)
    {
        var entry = new StreamEvent()
        {
            Event = e,
            EventId = e.ReadIdOrDefault()!,
            StreamId = streamId!,
            EventConcreteType = e.GetType()
        };

        return entry;
    }
    
    // public static StreamEvent ToStreamEvent<TEventId,TStreamId>(this ObjectEntry<TEventId,TStreamId> e, TStreamId streamId)
    // {
    //     var entry = new StreamEvent()
    //     {
    //         Event = e.SerializedValue,
    //         EventId = e.ReadIdOrDefault()!,
    //         StreamId = streamId!,
    //         EventConcreteType = e.GetType()
    //     };
    //
    //     return entry;
    // }
    
    public static void AddEvent<TStreamId>(this List<object> list, TStreamId streamId, object e)
    {
        var entry = e.ToStreamEvent(streamId);
        
        list.Add(entry);
    }
}