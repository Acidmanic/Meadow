using System;
using System.Security.Cryptography.X509Certificates;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Attributes;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Test.Functional.Models.EventStream;

// [EventStreamPreferences(typeof(Guid),typeof(Guid))]
// public record NumberEvent([UniqueMember] Guid EventId,double Number);

[EventStreamPreferences(typeof(long), typeof(long))]
public class NumberEvent
{
    [UniqueMember]
    [TreatAsLeaf]
    public long Id { get; set; }
    
    public double Number { get; set; }
}