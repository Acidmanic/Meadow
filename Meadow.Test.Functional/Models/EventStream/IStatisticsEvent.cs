using System;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Attributes;

namespace Meadow.Test.Functional.Models.EventStream;

// [EventStreamPreferences(typeof(Guid),typeof(Guid))]
// public record NumberEvent([UniqueMember] Guid EventId,double Number);

[EventStreamPreferences(typeof(Guid), typeof(long))]
public interface IStatisticsEvent
{
    [UniqueMember]
    [TreatAsLeaf]
    public long Id { get; set; }
    
}