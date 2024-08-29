using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Test.Functional.Models.EventStream;
using Meadow.Test.Functional.TestEnvironment;

namespace Meadow.Test.Functional.Suits.DataProviders;

public class StatisticsDataProvider:ICaseDataProvider
{

    public static readonly List<NumberEvent> Events = new List<NumberEvent>();

    public void Initialize()
    {
        Events.Add(new NumberEvent(Guid.NewGuid(),10));
        Events.Add(new NumberEvent(Guid.NewGuid(),20));
        Events.Add(new NumberEvent(Guid.NewGuid(),30));
        Events.Add(new NumberEvent(Guid.NewGuid(),40));

        SeedSet = new List<List<object>> { Events.Select(e => e as object).ToList() };
    }

    public void PostSeeding()
    {
    }

    public List<List<object>> SeedSet { get; private set; } = new();
}