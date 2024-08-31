namespace Meadow.Test.Functional.Models.EventStream;


public class NumberEvent:IStatisticsEvent
{
    
    public long Id { get; set; }
    
    public double Number { get; set; }
}