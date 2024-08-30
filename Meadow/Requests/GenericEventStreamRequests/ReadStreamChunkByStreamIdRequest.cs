using Meadow.Contracts;

namespace Meadow.Requests.GenericEventStreamRequests;

public sealed class ReadStreamChunkByStreamIdRequest<TEvent,TEventId, TStreamId> :
    EventStreamRequestBase<TEvent, TEventId, TStreamId,
        ReadStreamChunkByStreamIdRequest<TEvent, TEventId, TStreamId>.Argument>
{
    public class Argument
    {
        public TEventId BaseEventId { get; set; }
        public TStreamId StreamId { get; set; }

        public long Count { get; set; }
    }
    
    public ReadStreamChunkByStreamIdRequest(TStreamId streamId, TEventId baseEventId, long count) 
    {
        ToStorage = new Argument()
        {
            StreamId = streamId,
            BaseEventId = baseEventId,
            Count = count
        };
    }

    protected override string PickName(NameConvention nameConvention)
    {
        return nameConvention.ReadStreamChunkByStreamId;
    }
}