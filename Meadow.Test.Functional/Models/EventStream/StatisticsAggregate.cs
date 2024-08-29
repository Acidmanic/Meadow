using System;
using System.Reflection.Metadata;

namespace Meadow.Test.Functional.Models.EventStream;

public class StatisticsAggregate
{
    public double Count { get; set; }

    public double Sum { get; set; }

    public double Min { get; set; }

    public double Max { get; set; }

    public double Average { get; set; }
    
    public Guid Id { get; set; }

    public StatisticsAggregate()
    {
        Count = 0;
        Sum = 0;
        Min = double.MaxValue;
        Max = double.MinValue;
        Average = 0;
    }

    public void ReceiveNumber(double number)
    {
        Count++;

        Sum += number;

        if (number < Min) Min = number;
        if (number > Max) Max = number;

        Average = Sum / Count;
    }

    public Statistics GetValue()
    {
        return new Statistics
        {
            Average = Average,
            Count = Count,
            Max = Max,
            Min = Min,
            Sum = Sum
        };
    }
}