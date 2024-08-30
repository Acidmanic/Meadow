using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Test.Functional.Models.EventStream;
using Meadow.Test.Functional.TestEnvironment;

namespace Meadow.Test.Functional.Suits.DataProviders;

public class StatisticsDataProvider : ICaseDataProvider
{
    public static readonly List<NumberEvent> Events = new List<NumberEvent>();

    public static readonly Guid StreamId1 = Guid.Parse("15ff3d31-fcaf-40c2-8201-c01821ab4ced");
    public static Statistics Expected => new Statistics
    {
        Average = Events.Average(e => e.Number),
        Count = Events.Count,
        Max = Events.Max(e => e.Number),
        Min = Events.Min(e => e.Number),
        Sum = Events.Sum(e => e.Number)
    };
    
    

    public void Initialize()
    {
        // Events.Add(new NumberEvent{Id = Guid.NewGuid(),Number = 10});
        // Events.Add(new NumberEvent{Id = Guid.NewGuid(),Number = 20});
        // Events.Add(new NumberEvent{Id = Guid.NewGuid(),Number = 30});
        // Events.Add(new NumberEvent{Id = Guid.NewGuid(),Number = 40});
        Events.Add(new NumberEvent{Id = 1,Number = 10});
        Events.Add(new NumberEvent{Id = 2,Number = 20});
        Events.Add(new NumberEvent{Id = 3,Number = 30});
        Events.Add(new NumberEvent{Id = 4,Number = 40});

        SeedSet = new List<List<object>> { Events.Select(e => e.ToStreamEvent(StreamId1) as object).ToList() };
    }

    public void PostSeeding()
    {
    }

    public List<List<object>> SeedSet { get; private set; } = new();
}