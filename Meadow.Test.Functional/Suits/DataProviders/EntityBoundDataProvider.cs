using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TestEnvironment;

namespace Meadow.Test.Functional.Suits.DataProviders;

public class EntityBoundDataProvider : ICaseDataProvider
{
    public static readonly string MinimumName = "a";
    public static readonly string MaximumName = "d";
    public static readonly string MinimumSurname = "A";
    public static readonly string MaximumSurname = "D";
    public static readonly int MinimumAge = 25;
    public static readonly int MaximumAge = 100;

    public static readonly string[] ExistingNames = { "a", "b", "c", "d" };
    public static readonly string[] ExistingSurnames = { "A", "B", "C", "D" };
    public static readonly int[] ExistingAges = { 100, 75, 50, 25 };

    private static readonly List<Person> RangePersons = new()
    {
        new Person { Name = "a", Age = 100, Surname = "A" },
        new Person { Name = "b", Age = 75, Surname = "B" },
        new Person { Name = "c", Age = 50, Surname = "C" },
        new Person { Name = "d", Age = 25, Surname = "D" },
    };

    public void Initialize()
    {
    }

    public void PostSeeding()
    {
    }

    public List<List<object>> SeedSet { get; } = new() { new List<object>(RangePersons.Select(p => p as object)) };
}