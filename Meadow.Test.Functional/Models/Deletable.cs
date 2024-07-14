namespace Meadow.Test.Functional.Models;

public class Deletable
{
    public int Id { get; set; }

    public bool IsDeleted { get; set; } = false;
}