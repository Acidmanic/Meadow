using Meadow;
using Meadow.Contracts;
using Meadow.Requests.GenericEventStreamRequests;

namespace EnTier.DataAccess.Meadow.GenericEventStreamRequests;

public sealed class ReadAllStreamsRequest<TEvent, TEventId, TStreamId> :
    EventStreamRequestBase<TEvent, TEventId, TStreamId, MeadowVoid>
{

    protected override string PickName(NameConvention nameConvention)
    {
        return nameConvention.ReadAllStreams;
    }
}