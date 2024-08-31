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
public class AbstractEventStreamSuit : EventStreamSuit<IStatisticsEvent,long,NumberEvent,StatisticsDataProvider>
{
    public AbstractEventStreamSuit(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }
}

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class ConcreteClassEventStreamSuit : EventStreamSuit<NumberEventClass,long,NumberEventClass,ClassStatisticsDataProvider>
{
    public ConcreteClassEventStreamSuit(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }
}

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class ConcreteRecordEventStreamSuit : EventStreamSuit<NumberEventRecord,Guid,NumberEventRecord,RecordStatisticsDataProvider>
{
    public ConcreteRecordEventStreamSuit(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }
}

public abstract class EventStreamSuit<TEventBase,TEventId, TConcreteEvent,TDataProvider>
where TDataProvider:ICaseDataProvider, new()
{
    private const Databases Database = Databases.SqLite;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _scriptsDirectory = "SnippetComposedMacroScripts";
    

    private string EventStreamScriptContent => "-- {{WipEventStream " + typeof(TEventBase).FullName + "}}";

    public EventStreamSuit(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Should_SetupEngine_NoException()
    {
        var environment = CreateEnvironment();

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), _ => { });
    }

    [Fact]
    public void Should_Read_AllSeeded_Events()
    {
        var environment = CreateEnvironment();

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            var actual = c.EventStreamRead<TEventBase, TEventId, Guid>();

            var expected = c.Data.Events();

            Assert.Equal(expected.Count, actual.Count);
        });
    }

    [Fact]
    public void Should_Read_AllEvents_ForEachStreamId()
    {
        var environment = CreateEnvironment();

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            foreach (var eventsByStreamId in c.Data.EventsByStreamId)
            {
                var streamId = (Guid)eventsByStreamId.Key;

                var expected = eventsByStreamId.Value;
                var expectedEvents = expected.ToEvents<TConcreteEvent>();

                var actual = c.EventStreamRead<TEventBase, TEventId, Guid>(streamId);
                var actualEvents = actual.ToEvents<TConcreteEvent>();

                Assert.Equal(expected.Count, actual.Count);

                AssertX.ContainSameItemsDeep(expectedEvents, actualEvents);

                AssertX.AreInSameOrder(expectedEvents, actualEvents);
            }
        });
    }


    [Fact]
    public void Should_Read_AllEvents_AfterGivenBase()
    {
        var environment = CreateEnvironment();

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            var allSeededEvents = c.Data.Events();

            for (int baseIndex = 0; baseIndex < allSeededEvents.Count; baseIndex++)
            {
                for (int windowSize = 1; windowSize < allSeededEvents.Count; windowSize++)
                {
                    var baseEvent = allSeededEvents[baseIndex];
                    var expectedReadCount = Math.Min(allSeededEvents.Count - baseIndex - 1, windowSize);
                    var skip = allSeededEvents.IndexOf(baseEvent) + 1;
                    var expected = allSeededEvents.Skip(skip).Take(expectedReadCount).ToList();
                    var expectedEvents = expected.ToEvents<TConcreteEvent>();

                    var baseEventId = (TEventId)baseEvent.EventId;

                    var actual = c.EventStreamRead<TEventBase, TEventId, Guid>(baseEventId, windowSize);
                    var actualEvents = actual.ToEvents<TConcreteEvent>();

                    Assert.Equal(expectedReadCount, actual.Count);

                    AssertX.ContainSameItemsDeep(expectedEvents, actualEvents);

                    AssertX.AreInSameOrder(expectedEvents, actualEvents);

                    _testOutputHelper.WriteLine("[PASS] Expected {0} Items from Total: {1}", expectedReadCount,
                        allSeededEvents.Count);
                }
            }
        });
    }

    [Fact]
    public void Should_Read_AllEvents_AfterGivenBase_PerStreamId()
    {
        var environment = CreateEnvironment();

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

                        var expectedReadCount = Math.Min(allSeededEvents.Count - baseIndex - 1, windowSize);

                        var skip = allSeededEvents.IndexOf(baseEvent) + 1;
                        var expected = allSeededEvents.Skip(skip).Take(expectedReadCount).ToList();
                        var expectedEvents = expected.ToEvents<TConcreteEvent>();

                        var baseEventId = (TEventId)baseEvent.EventId;

                        var actual = c.EventStreamRead<TEventBase, TEventId, Guid>(streamId, baseEventId, windowSize);
                        var actualEvents = actual.ToEvents<TConcreteEvent>();

                        Assert.Equal(expectedReadCount, actual.Count);

                        AssertX.ContainSameItemsDeep(expectedEvents, actualEvents);

                        AssertX.AreInSameOrder(expectedEvents, actualEvents);

                        _testOutputHelper.WriteLine("[PASS] Expected {0} Items For Stream: {1}, Stream.Total: {2}",
                            expectedReadCount, streamId, allSeededEvents.Count);
                    }
                }
            }
        });
    }

    private Environment<TDataProvider> CreateEnvironment()
    {
        var environment = new Environment<TDataProvider>(_scriptsDirectory,GetType().Name);
        
        environment.OverrideScriptFile("0006-EventStream.sql",EventStreamScriptContent);

        return environment;
    }
}

internal static class EventStreamConversions
{
    public static List<T> ToEvents<T>(this IEnumerable<StreamEvent> streamEvents)
    {
        return streamEvents.Select(s => (T)s.Event).ToList();
    }
}