using System;
using System.Reflection;
using Acidmanic.Utilities.Results;
using Meadow.Attributes;

namespace Meadow.Scaffolding.Attributes;

public class EventStreamPreferencesInfo
{
    public Type StreamIdType { get; set; }

    public Type EventIdType { get; set; }

    public long MaximumTypeNameLength { get; set; }

    public long MaximumDataSize { get; set; }

    public Type EventAbstraction { get; set; }

    public static Result<EventStreamPreferencesInfo> FromType<T>()
    {
        return FromType(typeof(T));
    }

    public static Result<EventStreamPreferencesInfo> FromType(Type eventType)
    {
        var attribute = eventType.GetCustomAttribute<EventStreamPreferencesAttribute>();

        if (attribute != null)
        {
            return new Result<EventStreamPreferencesInfo>(true, new EventStreamPreferencesInfo
            {
                EventAbstraction = eventType,
                EventIdType = attribute.EventId,
                MaximumDataSize = attribute.MaximumDataSize,
                MaximumTypeNameLength = attribute.MaximumTypeNameLength,
                StreamIdType = attribute.StreamIdType
            });
        }

        return new Result<EventStreamPreferencesInfo>().FailAndDefaultValue();
    }
}