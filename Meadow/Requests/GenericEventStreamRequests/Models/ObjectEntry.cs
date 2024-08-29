using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Requests.GenericEventStreamRequests.Models
{
    public class ObjectEntry<TEventId, TStreamId>
    {
        public ObjectEntry(TEventId eventId, TStreamId streamId, string typeName, string serializedValue)
        {
            EventId = eventId;
            StreamId = streamId;
            TypeName = typeName;
            SerializedValue = serializedValue;
        }

        public ObjectEntry() : this(default, default, "System.object", "")
        {
        }

        [AutoValuedMember] [UniqueMember] public virtual TEventId EventId { get; set; }

        public TStreamId StreamId { get; set; }

        public string TypeName { get; set; }

        public string SerializedValue { get; set; }
    }
}