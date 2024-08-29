using System;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Test.Functional.Models.EventStream;

[EventStreamPreferences(typeof(Guid),typeof(Guid))]
public record NumberEvent([UniqueMember] Guid EventId,double Number);