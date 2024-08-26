using System;

namespace Meadow.Test.Functional.Models.Events;

public record StartedEventData(string Title, DateTime Date);
public record FinishedEventData(string Title, DateTime Date);