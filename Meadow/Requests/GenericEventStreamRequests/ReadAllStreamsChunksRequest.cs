using Meadow.Contracts;
using Meadow.Extensions;

namespace Meadow.Requests.GenericEventStreamRequests;

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
    private NameConvention NameConvention => Configuration.GetNameConvention<TEvent>();
    
    public override string RequestText
    {
        get => PickName(NameConvention);
        protected set { }
    }
}