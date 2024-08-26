using System;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Test.Functional.Models;

[EventStreamPreferences(typeof(Guid),typeof(Guid))]
public class BigEvent
{
    [UniqueMember]
    public Guid EventId { get; set; }
    
}