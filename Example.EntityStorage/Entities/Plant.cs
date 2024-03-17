using System;
using Acidmanic.Utilities.Reflection.Attributes;
using Example.EntityStorage.ValueObjects;


namespace Example.EntityStorage.Entities;

public class Plant
{
    [TreatAsLeaf]
    public PlantName Name { get; set; }
    
    [TreatAsLeaf]
    public DateTime CreateDate { get; set; }
    
    [UniqueMember]
    public Guid Id { get; set; }
    
    
    public PlantType Type { get; set; }

    public static Plant Create(string name, PlantType type = null)
    {
        type ??= PlantTypes.General;
        
        return new Plant
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreateDate = DateTime.Now,
            Type = type
        };
    }
}