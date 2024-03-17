using System;
using Acidmanic.Utilities.Reflection.Attributes;
using Org.BouncyCastle.Crypto.Engines;

namespace Example.EntityStorage.ValueObjects;

[AlteredType(typeof(string))]
public sealed record PlantType(Guid Id, string TypeName, int OrderIndex)
{
    [UniqueMember]
    public Guid Id { get; private set; } = Id;
}


public static class PlantTypes
{

    public static PlantType General { get; } = new PlantType(Guid.Parse("76171c60-e48a-11ee-8bb2-c325cfc96a14"), "General Plant", 1);
    public static PlantType Flower { get; } = new PlantType(Guid.Parse("85a24ea2-e48a-11ee-b799-5386e16dabc3"), "Flower", 1);
    public static PlantType Vegetable { get; } = new PlantType(Guid.Parse("89942f3a-e48a-11ee-ba8b-770275b84573"),"Vegetable", 2);

    public static PlantType[] Items = { General,Flower,Vegetable };
}