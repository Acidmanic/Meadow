using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.EntityStorage.ValueObjects;

[AlteredType(typeof(string))]
public sealed record PlantType(string TypeName,int Order);


public static class PlantTypes
{

    public static PlantType General { get; } = new PlantType("General Plant", 1);
    public static PlantType Flower { get; } = new PlantType("Flower", 1);
    public static PlantType Vegetable { get; } = new PlantType("Vegetable", 2);
}