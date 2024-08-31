using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Requests.GenericEventStreamRequests.Models;
using Meadow.Test.Functional.Models.EventStream;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;


[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class EventStreamSuit
{
    private const Databases Database = Databases.SqLite;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _scriptsDirectory = "SnippetComposedMacroScripts";

    public EventStreamSuit(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Should_SetupEngine_NoException()
    {
        var environment = new Environment<StatisticsDataProvider>(_scriptsDirectory);

        var actualAggregate = new StatisticsAggregate() { Id = Guid.NewGuid() };

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c => { });
    }

    [Fact]
    public void Should_Read_AllSeeded_Events()
    {
        var environment = new Environment<StatisticsDataProvider>(_scriptsDirectory);

        var actualAggregate = new StatisticsAggregate() { Id = Guid.NewGuid() };

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            var actual = c.EventStreamRead<NumberEvent, long, Guid>();

            var expected = c.Data.Events();

            Assert.Equal(expected.Count, actual.Count);
        });
    }

    [Fact]
    public void Should_Read_AllEvents_ForEachStreamId()
    {
        var environment = new Environment<StatisticsDataProvider>(_scriptsDirectory);

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            foreach (var eventsByStreamId in c.Data.EventsByStreamId)
            {
                var streamId = (Guid)eventsByStreamId.Key;

                var expected = eventsByStreamId.Value;
                var expectedEvents = expected.ToEvents<NumberEvent>();

                var actual = c.EventStreamRead<NumberEvent, long, Guid>(streamId);
                var actualEvents = actual.ToEvents<NumberEvent>();
                
                Assert.Equal(expected.Count, actual.Count);
                
                AssertX.ContainSameItemsDeep(expectedEvents, actualEvents, e=> $"Value:{e.Number}");
                
                AssertX.AreInSameOrder(expectedEvents, actualEvents, e=> $"Value:{e.Number}");
            }
        });
    }
    
    
    [Fact]
    public void Should_Read_AllEvents_AfterGivenBase()
    {
        var environment = new Environment<StatisticsDataProvider>(_scriptsDirectory);

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {

            var allSeededEvents = c.Data.Events();

            for (int baseIndex = 0; baseIndex < allSeededEvents.Count; baseIndex++)
            {

                for (int windowSize = 1; windowSize < allSeededEvents.Count; windowSize++)
                {
                    var baseEvent = allSeededEvents[baseIndex];
                    var expectedReadCount = Math.Min(allSeededEvents.Count - baseIndex-1,windowSize);
                    var skip = allSeededEvents.IndexOf(baseEvent) + 1;
                    var expected = allSeededEvents.Skip(skip).Take(expectedReadCount).ToList();
                    var expectedEvents = expected.ToEvents<NumberEvent>();
                    
                    var baseEventId = (long)baseEvent.EventId;

                    var actual = c.EventStreamRead<NumberEvent, long, Guid>(baseEventId,windowSize);
                    var actualEvents = actual.ToEvents<NumberEvent>();
                    
                    Assert.Equal(expectedReadCount,actual.Count);
                    
                    AssertX.ContainSameItemsDeep(expectedEvents, actualEvents, e=> $"Value:{e.Number}");
                    
                    AssertX.AreInSameOrder(expectedEvents, actualEvents, e=> $"Value:{e.Number}");
                    
                    _testOutputHelper.WriteLine("[PASS] Expected {0} Items from Total: {1}", expectedReadCount,allSeededEvents.Count);
                }
            }
        });
    }
    
    [Fact]
    public void Should_Read_AllEvents_AfterGivenBase_PerStreamId()
    {
        var environment = new Environment<StatisticsDataProvider>(_scriptsDirectory);

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            
            foreach (var eventsByStreamId in c.Data.EventsByStreamId)
            {
                var allSeededEvents = eventsByStreamId.Value;
                var streamId = (Guid)eventsByStreamId.Key;

                for (int baseIndex = 0; baseIndex < allSeededEvents.Count; baseIndex++)
                {

                    for (int windowSize = 1; windowSize < allSeededEvents.Count; windowSize++)
                    {
                        var baseEvent = allSeededEvents[baseIndex];
                        
                        var expectedReadCount = Math.Min(allSeededEvents.Count - baseIndex-1,windowSize);
                        
                        var skip = allSeededEvents.IndexOf(baseEvent) + 1;
                        var expected = allSeededEvents.Skip(skip).Take(expectedReadCount).ToList();
                        var expectedEvents = expected.ToEvents<NumberEvent>();
                    
                        var baseEventId = (long)baseEvent.EventId;

                        var actual = c.EventStreamRead<NumberEvent, long, Guid>(streamId, baseEventId,windowSize);
                        var actualEvents = actual.ToEvents<NumberEvent>();
                    
                        Assert.Equal(expectedReadCount,actual.Count);
                    
                        AssertX.ContainSameItemsDeep(expectedEvents, actualEvents, e=> $"Value:{e.Number}");
                        
                        AssertX.AreInSameOrder(expectedEvents, actualEvents, e=> $"Value:{e.Number}");
                        
                        _testOutputHelper.WriteLine("[PASS] Expected {0} Items For Stream: {1}, Stream.Total: {2}",
                            expectedReadCount,streamId,allSeededEvents.Count);
                    }
                }
            }
            
        });
    }
    
}

internal static class EventStreamConversions
{

    public static List<T> ToEvents<T>(this IEnumerable<StreamEvent> streamEvents)
    {
        return streamEvents.Select(s => (T)s.Event).ToList();
    }
} 