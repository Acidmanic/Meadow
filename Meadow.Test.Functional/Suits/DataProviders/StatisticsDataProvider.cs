using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Test.Functional.Models.EventStream;
using Meadow.Test.Functional.TestEnvironment;

namespace Meadow.Test.Functional.Suits.DataProviders;

public class StatisticsDataProvider : ICaseDataProvider
{

    public static readonly Guid StreamId1 = Guid.Parse("15ff3d31-fcaf-40c2-8201-c01821ab4ced");
    public static readonly Guid StreamId2 = Guid.Parse("134e5f13-5ba5-4ff2-89ef-8ce70f802c26");

    public static readonly List<Guid> SeedStreamIds = new List<Guid>() { StreamId1,StreamId2};

    public static Statistics Expected(List<object> list)
    {
        var events = list.Select(i => (i as NumberEvent)!).ToList();
        
        return new Statistics
        {
            Average = events.Average(e => e.Number),
            Count = events.Count,
            Max = events.Max(e => e.Number),
            Min = events.Min(e => e.Number),
            Sum = events.Sum(e => e.Number)
        };
    }
    
    

    public void Initialize()
    {

        var list1 = new List<object>();
        
        list1.AddEvent(StreamId1,new NumberEvent{Id = 1,Number = 10});
        list1.AddEvent(StreamId1,new NumberEvent{Id = 2,Number = 20});
        list1.AddEvent(StreamId1,new NumberEvent{Id = 3,Number = 30});
        list1.AddEvent(StreamId1,new NumberEvent{Id = 4,Number = 40});
        
        var list2 = new List<object>();
        
        list2.AddEvent(StreamId2,new NumberEvent{Id = 5,Number = 50});
        list2.AddEvent(StreamId2,new NumberEvent{Id = 6,Number = 60});

       SeedSet = new List<List<object>>();
       
       SeedSet.Add(list1);
       SeedSet.Add(list2);
        
    }

    public void PostSeeding()
    {
    }

    public List<List<object>> SeedSet { get; private set; } = new();
}