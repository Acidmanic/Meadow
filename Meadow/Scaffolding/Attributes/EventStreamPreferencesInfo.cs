using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Acidmanic.Utilities.Results;
using Meadow.Attributes;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;

namespace Meadow.Scaffolding.Attributes;

public class EventStreamPreferencesInfo
{
    public Type StreamIdType { get; set; }

    public Type EventIdType { get; set; }

    public long MaximumTypeNameLength { get; set; }

    public long MaximumDataSize { get; set; }

    public Type EventAbstraction { get; set; }
    
    public Type EventType { get; set; }
    
    

    public static Result<EventStreamPreferencesInfo> FromType<T>()
    {
        return FromType(typeof(T));
    }

    public static Result<EventStreamPreferencesInfo> FromType(Type eventType)
    {
        var attribute = eventType.GetHierarchicalCustomAttribute<EventStreamPreferencesAttribute>();

        if (attribute)
        {
            return new Result<EventStreamPreferencesInfo>(true, new EventStreamPreferencesInfo
            {
                EventType = eventType,
                EventAbstraction = attribute.Secondary,
                EventIdType = attribute.Primary.EventId,
                MaximumDataSize = attribute.Primary.MaximumDataSize,
                MaximumTypeNameLength = attribute.Primary.MaximumTypeNameLength,
                StreamIdType = attribute.Primary.StreamIdType
            });
        }

        return new Result<EventStreamPreferencesInfo>().FailAndDefaultValue();
    }
 
    
}