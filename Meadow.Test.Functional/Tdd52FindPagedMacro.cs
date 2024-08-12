using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional;

public class Tdd52FindPagedMacro : PersonUseCaseTestBase
{
    protected override void SelectDatabase()
    {
        UseSqlServer();
    }


    protected override MeadowConfiguration RegulateConfigurations(MeadowConfiguration configurations)
    {
        configurations.AddFilter<Deletable>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
        configurations.AddFilter<Person>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
        configurations.AddFilter<Job>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
        configurations.AddFilter<Address>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));


        return configurations;
    }


    protected override void Main(MeadowEngine engine, ILogger logger)
    {
        var allPersonResponse = engine
            .PerformRequest(new FindPagedRequest<Person, long>(new FilterQuery()));

        if (allPersonResponse.Failed)
        {
            throw allPersonResponse.FailureException;
        }

        var allPersons = allPersonResponse.FromStorage;

        if (allPersons.Count != Persons.Length)
        {
            throw new Exception("Problem with reading all records in find pages");
        }


        FindPagedMustBeAbleToPerformCorrectPagination(engine, logger);

        logger.LogInformation("[PASS] Pagination Is Ok");

        FindPagedMustFindRecords(engine, b =>
            b.Where(p => p.Age).IsLargerThan(50), GetPerson(p => p.Age > 50));

        FindPagedMustFindRecords(engine, b =>
            b.Where(p => p.Age).IsSmallerThan(50), GetPerson(p => p.Age < 50));

        FindPagedMustFindRecords(engine, b =>
            b.Where(p => p.Name).IsEqualTo("Mani"), GetPerson(p => p.Name == "Mani"));

        FindPagedMustFindRecords(engine, b =>
                b.Where(p => p.Name).IsEqualTo("Mina", "Farshid"),
            GetPerson(p => p.Name == "Mina" || p.Name == "Farshid"));

        FindPagedMustFindRecords(engine, b =>
                b.Where(p => p.Name).IsNotEqualTo("Mina", "Farshid"),
            GetPerson(p => p.Name != "Mina" && p.Name != "Farshid"));

        logger.LogInformation("[PASS] Filter Is Ok");

        FindPagedMustPerformCorrectSorting(engine, Sort(Persons, (p1, p2) => p1.Age - p2.Age),
            o => o.OrderAscendingBy(p => p.Age));

        FindPagedMustPerformCorrectSorting(engine, Sort(Persons, (p1, p2) => p2.Age - p1.Age),
            o => o.OrderDescendingBy(p => p.Age));

        FindPagedMustPerformCorrectSorting(engine, Sort(Persons, (p1, p2) => string.Compare(p1.Name, p2.Name, StringComparison.Ordinal)),
            o => o.OrderAscendingBy(p => p.Name));

        FindPagedMustPerformCorrectSorting(engine, Sort(Persons, (p1, p2) => string.Compare(p2.Name, p1.Name, StringComparison.Ordinal)),
            o => o.OrderDescendingBy(p => p.Name));

        logger.LogInformation("[PASS] Ordering Is Ok");

        Index<Person, long>(engine, Persons);

        FindPagedMustFindExpectedItemsForGivenSearchTerms(engine,
            Choose(Persons.FirstOrDefault(p => p.Name=="Farshid")),"farshid");
        
        FindPagedMustFindExpectedItemsForGivenSearchTerms(engine,
            Choose(Persons.Where(p => 
                p.Name=="Mani" ||
                p.Name=="Mona" ||
                p.Name=="Farshid"
                )),"Moai");
        
        FindPagedMustFindExpectedItemsForGivenSearchTerms(engine,
            Choose(Persons.Where(p => 
                p.Name=="Farshid" ||
                p.Name=="Farimehr"
            )),"Far");
        
        logger.LogInformation("[PASS] Searching Is Ok");
    }

    private List<T> Choose<T>(params T[] items) => new List<T>(items);
    
    private List<T> Choose<T>(IEnumerable<T> items) => new List<T>(items);
    
    private void FindPagedMustFindExpectedItemsForGivenSearchTerms(MeadowEngine engine, List<Person> expectedResult, params string[] searchTerms )
    {
        var terms = Transliterate(searchTerms);

        var found = engine
            .PerformRequest(new FindPagedRequest<Person, long>(new FilterQuery(),0,1000,terms))
            .FromStorage;

        foreach (var person in expectedResult)
        {
            AssertContainShallow(found, person, $"Person: {person.Name} was not found in the result");
        }

        AssertSameSize(expectedResult, found, "Extra Items has been read from database");
    }

    private List<TModel> Sort<TModel>(IEnumerable<TModel> items, Comparison<TModel> compare)
    {
        var list = new List<TModel>(items);

        list.Sort(compare);

        return list;
    }

    private void FindPagedMustFindRecords(MeadowEngine engine, Action<FilterQueryBuilder<Person>> select,
        params Person[] items)
    {
        var queryBuilder = new FilterQueryBuilder<Person>();

        select(queryBuilder);

        var query = queryBuilder.Build();

        var found = engine
            .PerformRequest(new FindPagedRequest<Person, long>(query))
            .FromStorage;

        foreach (var person in items)
        {
            AssertContainShallow(found, person, $"Person: {person.Name} was not found in the result");
        }

        AssertSameSize(items, found, "Extra Items has been read from database");
    }


    private void FindPagedMustPerformCorrectSorting(MeadowEngine engine, List<Person> expected, Action<OrderSetBuilder<Person>> buildOrders)
    {
        var orderBuilder = new OrderSetBuilder<Person>();

        buildOrders(orderBuilder);

        var orders = orderBuilder.Build();

        var found = engine
            .PerformRequest(new FindPagedRequest<Person, long>(new FilterQuery(), 0, 1000, null, orders))
            .FromStorage;

        AssertSameSize(expected, found, "Read items does not match with expectations");

        AssertSameOrder(expected, found);
    }

    private void AssertSameOrder(List<Person> expected, List<Person> actual)
    {
        for (int i = 0; i < expected.Count; i++)
        {
            if (!AreEqualShallow(expected[i], actual[i]))
            {
                throw new Exception($"Expected to find {expected[i].Name}:{expected[i].Id} at {i}'th place, but found {actual[i].Name}:{actual[i].Id}");
            }
        }
    }

    private void AssertSameSize(IEnumerable<Person> s1, IEnumerable<Person> s2, string message)
    {
        if (s1.Count() != s2.Count())
        {
            throw new Exception(message);
        }
    }

    private void AssertContainShallow(List<Person> found, Person person, string message)
    {
        if (found.All(f => !AreEqualShallow(f, person)))
        {
            throw new Exception(message);
        }
    }

    private bool AreEqualShallow(Person p1, Person p2)
    {
        return p1.Name == p2.Name &&
               p1.IsDeleted == p2.IsDeleted &&
               p1.Age == p2.Age &&
               p1.Surname == p2.Surname &&
               p1.JobId == p2.JobId;
    }

    private void FindPagedMustBeAbleToPerformCorrectPagination(MeadowEngine engine, ILogger logger)
    {
        var all = engine
            .PerformRequest(new FindPagedRequest<Person, long>(new FilterQuery(), 0, 2))
            .FromStorage;


        var size = 2;

        for (int offset = 0; offset < all.Count; offset += size)
        {
            var read = engine
                .PerformRequest(new FindPagedRequest<Person, long>(new FilterQuery(), 0, 2))
                .FromStorage;

            if (read.Count != size)
            {
                throw new Exception("Pagination size issue");
            }

            if (read[offset].Id != all[offset].Id || read[offset + 1].Id != all[offset + 1].Id)
            {
                throw new Exception("Pagination offset issue");
            }
        }
    }
}