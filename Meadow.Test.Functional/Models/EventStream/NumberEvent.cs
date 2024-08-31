using System;
using Acidmanic.Utilities.Extensions;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Attributes;

namespace Meadow.Test.Functional.Models.EventStream;


[EventStreamSerializationCompression(Compressions.GZip)]
[EventStreamPreferences(typeof(Guid), typeof(Guid))]
public record NumberEventRecord([UniqueMember] [TreatAsLeaf] Guid Id, double Number)
{
    public override string ToString() => $"{Id}:{Number}";
};


[EventStreamSerializationCompression(Compressions.GZip)]
[EventStreamPreferences(typeof(Guid), typeof(long))]
public class NumberEventClass
{
    
    [UniqueMember]
    public long Id { get; set; }
    
    public double Number { get; set; }

    public override string ToString() => $"{Id}:{Number}";
}

public class NumberEvent:IStatisticsEvent
{
    
    public long Id { get; set; }
    
    public double Number { get; set; }
    
    public override string ToString() => $"{Id}:{Number}";
}