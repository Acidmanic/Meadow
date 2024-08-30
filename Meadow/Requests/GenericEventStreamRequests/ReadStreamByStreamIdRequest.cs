using Meadow.Contracts;

namespace Meadow.Requests.GenericEventStreamRequests;

public sealed class ReadStreamByStreamIdRequest<TEvent, TEventId, TStreamId> :
    EventStreamRequestBase<TEvent, TEventId, TStreamId,
        ReadStreamByStreamIdRequest<TEvent, TEventId, TStreamId>.Argument>
{
    public class Argument
    {
        public TStreamId StreamId { get; set; }
    }

    public ReadStreamByStreamIdRequest(TStreamId streamId) : base()
    {
        ToStorage = new Argument() { StreamId = streamId };
    }

    protected override string PickName(NameConvention nameConvention)
    {
        return nameConvention.ReadStreamByStreamId;
    }
}