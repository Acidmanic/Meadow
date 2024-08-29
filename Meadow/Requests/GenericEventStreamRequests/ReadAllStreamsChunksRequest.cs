
using Meadow.Contracts;
using Meadow.Requests.GenericEventStreamRequests;

namespace EnTier.DataAccess.Meadow.GenericEventStreamRequests;

public sealed class ReadAllStreamsChunksRequest<TEvent,TEventId, TStreamId> :
    EventStreamRequestBase<TEvent, TEventId, TStreamId,
        ReadAllStreamsChunksRequest<TEvent, TEventId, TStreamId>.Argument>
{
    public class Argument
    {
        public TEventId BaseEventId { get; set; }
        public long Count { get; set; }
    }


    public ReadAllStreamsChunksRequest(TEventId baseEventId, long count) 
    {
        ToStorage = new Argument()
        {
            BaseEventId = baseEventId,
            Count = count
        };
    }

    protected override string PickName(NameConvention nameConvention)
    {
        return nameConvention.ReadAllStreamsChunks;
    }
}