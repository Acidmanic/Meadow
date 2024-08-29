using System;

namespace Meadow.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class EventStreamPreferencesAttribute : Attribute
{
    public Type StreamIdType { get; }

    public Type EventId { get; }

    public long MaximumTypeNameLength { get; }

    public long MaximumDataSize { get; }


    public EventStreamPreferencesAttribute(Type streamIdType, Type eventId,
        long maximumTypeNameLength = 256,
        long maximumDataSize = 256)
    {
        StreamIdType = streamIdType;
        EventId = eventId;
        MaximumTypeNameLength = maximumTypeNameLength;
        MaximumDataSize = maximumDataSize;
    }
}