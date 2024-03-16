using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.EntityStorage.ValueObjects;

[AlteredType(typeof(string))]
public struct PlantName
{
    

    public override bool Equals(object obj)
    {
        return obj is PlantName other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (Value != null ? Value.GetHashCode() : 0);
    }

    public string Value { get; }
    
    public PlantName(string value)
    {
        Value = value;
    }

    public static implicit operator string(PlantName value) => value.Value;

    public static implicit operator PlantName(string value) => new PlantName(value);

    public static bool operator ==(PlantName p1, PlantName p2)
    {
        return p1.Value == p2.Value;
    }

    public static bool operator !=(PlantName p1, PlantName p2)
    {
        return (p1.Value != p2.Value);
    }
    
    public bool Equals(PlantName other)
    {
        return Value == other.Value;
    }
    
}