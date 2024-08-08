using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models;

public class Deletable
{
    [UniqueMember]
    public int Id { get; set; }
    
    public int ClientNumber { get; set; }

    public bool IsDeleted { get; set; } = false;

    public string Information { get; set; } = "Default Value";
}