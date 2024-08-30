using System;
using System.Text;
using Meadow.Attributes;
using Meadow.Test.Functional.Models.EventStream;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Meadow.Utility;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

public class EventStreamSuit
{

    [EventStreamSerializationEncoding("utf-8")]
    private class Shit
    {
    }

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

        var actualAggregate = new StatisticsAggregate(){Id = Guid.NewGuid()};

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
           
        });
    }
    
    [Fact]
    public void Should_Read_AllSeeded_Events()
    {
        var environment = new Environment<StatisticsDataProvider>(_scriptsDirectory);

        var actualAggregate = new StatisticsAggregate(){Id = Guid.NewGuid()};

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            var entries = c.EventStreamRead<NumberEvent, long, Guid>().ToStreamEvent(c.MeadowConfiguration);

            var allEvents = c.Data.Get<NumberEvent>();
            
            Assert.Equal(allEvents.Count,entries.Count);
        });
    }
}