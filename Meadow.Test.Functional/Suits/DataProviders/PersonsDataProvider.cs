using System.Collections.Generic;
using System.Linq;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TestEnvironment;

namespace Meadow.Test.Functional.Suits.DataProviders;

public class PersonsDataProvider : ICaseDataProvider
{
    public void Initialize()
    {
        SeedSet.Clear();

        SeedSet.Add(new List<object>(_jobs));
        SeedSet.Add(new List<object>(_persons));
        SeedSet.Add(new List<object>(_addresses));
        SeedSet.Add(new List<object>(_tags));
    }

    public void PostSeeding() => PlugDataRelations();

    public List<List<object>> SeedSet { get; } = new();

    private static Job J(int id,string personName, long income)
    {
        return new Job
        {
            Id = id,
            Title = personName + "'s Job",
            JobDescription = personName + "'s job description",
            IncomeInRials = income
        };
    }

    private static Person P(string name, string surname, int age, long jobId, bool isDeleted = false)
    {
        return new Person
            { Age = age, Name = name, Surname = surname, JobId = jobId, IsDeleted = isDeleted };
    }

    private static Address A(int addressNumber, long personId)
    {
        string[] counts = { "First", "Second", "Third", "Fourth", "Fifth" };
        addressNumber -= 1;
        return new Address
        {
            Block = addressNumber,
            City = counts[addressNumber] + " City",
            Plate = addressNumber,
            Street = counts[addressNumber] + " Street",
            AddressName = counts[addressNumber] + " Address For " + personId,
            PersonId = personId
        };
    }

    private readonly Job[] _jobs =
    {
        J(1,"Mani", 100), J(2,"Mona", 200), J(3,"Mina", 300),
        J(4,"Farshid", 400), J(5,"Farimehr", 500), J(6,"Deleted", -100)
    };

    private readonly Person[] _persons =
    {
        P("Mani", "Moayedi", 37, 1),
        P("Mona", "Moayedi", 42, 2),
        P("Mina", "Haddadi", 56, 3),
        P("Farshid", "Moayedi", 63, 4),
        P("Farimehr", "Ayerian", 21, 5),
        P("Deleted", "Deletian", 128, 6, true),
    };

    private readonly Address[] _addresses =
    {
        A(1, 1),
        A(1, 2), A(2, 2),
        A(1, 3), A(2, 3), A(3, 3),
        A(1, 4), A(2, 4), A(3, 4), A(4, 4),
        A(1, 5), A(2, 5), A(3, 5), A(4, 5), A(5, 5),
        A(1, 6)
    };

    private readonly Tag[] _tags =
    {
        new () { PropertyId = 10, ProductClassId = 100 },
        new () { PropertyId = 20, ProductClassId = 200 },
        new () { PropertyId = 30, ProductClassId = 300 },
    };


    private void PlugDataRelations()
    {
        foreach (var person in _persons)
        {
            person.Addresses = _addresses.Where(a => a.PersonId == person.Id).ToList();

            person.Job = _jobs.First(j => person.JobId == j.Id);
        }
    }
}