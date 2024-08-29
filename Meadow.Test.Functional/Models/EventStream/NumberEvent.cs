using System;
using System.Security.Cryptography.X509Certificates;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Test.Functional.Models.EventStream;

// [EventStreamPreferences(typeof(Guid),typeof(Guid))]
// public record NumberEvent([UniqueMember] Guid EventId,double Number);

[EventStreamPreferences(typeof(Guid), typeof(Guid))]
public class NumberEvent
{
    [UniqueMember] public Guid EventId { get; set; }
    
    public double Number { get; set; }
}