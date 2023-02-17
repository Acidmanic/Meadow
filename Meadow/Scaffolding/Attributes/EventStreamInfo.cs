using System;
using System.Reflection;
using Acidmanic.Utilities.Results;

namespace Meadow.Scaffolding.Attributes;

public class EventStreamInfo
{
    public Type StreamIdType { get; set; }

    public Type EventIdType { get; set; }

    public long MaximumTypeNameLength { get; set; }

    public long MaximumDataSize { get; set; }

    public Type EventAbstraction { get; set; }

    public static Result<EventStreamInfo> FromType<T>()
    {
        return FromType(typeof(T));
    }

    public static Result<EventStreamInfo> FromType(Type eventType)
    {
        var attribute = eventType.GetCustomAttribute<EventStreamPreferencesAttribute>();

        if (attribute != null)
        {
            return new Result<EventStreamInfo>(true, new EventStreamInfo
            {
                EventAbstraction = eventType,
                EventIdType = attribute.EventId,
                MaximumDataSize = attribute.MaximumDataSize,
                MaximumTypeNameLength = attribute.MaximumTypeNameLength,
                StreamIdType = attribute.StreamIdType
            });
        }

        return new Result<EventStreamInfo>().FailAndDefaultValue();
    }
}