using System;
using Acidmanic.Utilities.Extensions;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Attributes;

namespace Meadow.Test.Functional.Models.EventStream;


[EventStreamSerializationCompression(Compressions.GZip)]
[EventStreamPreferences(typeof(Guid), typeof(long))]
public interface IStatisticsEvent
{
    [UniqueMember]
    public long Id { get; set; }
    
}