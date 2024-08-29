using System;

namespace Meadow.Requests.GenericEventStreamRequests.Models
{
    public class StreamEvent<TEvent, TEventId, TStreamId>
    {
        public TEvent Event { get; set; }

        public TStreamId StreamId { get; set; }

        public TEventId EventId { get; set; }
        
        public Type EventConcreteType { get; set; }
    }
}