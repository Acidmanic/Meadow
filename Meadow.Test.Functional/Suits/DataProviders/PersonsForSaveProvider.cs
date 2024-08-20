using System.Collections.Generic;
using System.Linq;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TestEnvironment;

namespace Meadow.Test.Functional.Suits.DataProviders;

public class PersonsForSaveProvider : ICaseDataProvider
{
    public void Initialize()
    {
        SeedSet.Clear();
        SeedSet.Add(new List<object>(persons));
    }

    public void PostSeeding() { }

    public List<List<object>> SeedSet { get; } = new();

    protected static Person P(string name, string surname, int age, long jobId, bool isDeleted = false)
    {
        return new Person
            { Age = age, Name = name, Surname = surname, JobId = jobId, IsDeleted = isDeleted };
    }

    private readonly Person[] persons =
    {
        P("Mani", "Moayedi", 37, 1),
        P("Mona", "Moayedi", 42, 2),
        P("Mina", "Haddadi", 56, 3),
        P("Farshid", "Moayedi", 63, 4),
        P("Farimehr", "Ayerian", 21, 5),
        P("Deleted", "Deletian", 128, 6, true),
    };
}