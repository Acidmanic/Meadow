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

        SeedSet.Add(new List<object>(jobs));
        SeedSet.Add(new List<object>(persons));
        SeedSet.Add(new List<object>(addresses));
    }

    public void PostSeeding() => PlugDataRelations();

    public List<List<object>> SeedSet { get; } = new();

    protected static Job J(string personName, long income)
    {
        return new Job
        {
            Title = personName + "'s Job",
            JobDescription = personName + "'s job description",
            IncomeInRials = income
        };
    }

    protected static Person P(string name, string surname, int age, long jobId, bool isDeleted = false)
    {
        return new Person
            { Age = age, Name = name, Surname = surname, JobId = jobId ,IsDeleted = isDeleted};
    }

    protected static Address A(int addressNumber, long personId)
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

    private readonly Job[] jobs =
        { J("Mani", 100), J("Mona", 200), J("Mina", 300),
            J("Farshid", 400), J("Farimehr", 500) , J("Deleted",-100)};

    private readonly Person[] persons =
    {
        P("Mani", "Moayedi", 37, 1),
        P("Mona", "Moayedi", 42, 2),
        P("Mina", "Haddadi", 56, 3),
        P("Farshid", "Moayedi", 63, 4),
        P("Farimehr", "Ayerian", 21, 5),
        P("Deleted", "Deletian", 128, 6,true),
    };

    private readonly Address[] addresses =
    {
        A(1, 1),
        A(1, 2), A(2, 2),
        A(1, 3), A(2, 3), A(3, 3),
        A(1, 4), A(2, 4), A(3, 4), A(4, 4),
        A(1, 5), A(2, 5), A(3, 5), A(4, 5), A(5, 5),
        A(1, 6)
    };


    private void PlugDataRelations()
    {
        foreach (var person in persons)
        {
            person.Addresses = addresses.Where(a => a.PersonId == person.Id).ToList();

            person.Job = jobs.FirstOrDefault(j => person.JobId == j.Id);
        }
    }
}