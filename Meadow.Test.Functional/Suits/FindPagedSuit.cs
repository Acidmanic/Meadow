using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TestEnvironment;
using Xunit;

namespace Meadow.Test.Functional.Suits;

public class FindPagedSuit
{
    private const Databases databases = Databases.MySql;

    private readonly Func<Person, string> personIdentifier = p => $"{p.Name}:{p.Id}";


    [Fact]
    public void Must_Filter_Persons_OlderThan50() =>
        FindPagedMustFilterRecords(qb => qb.Where(p => p.Age).IsLargerThan(50), p => p.Age > 50);

    [Fact]
    public void Must_Filter_Persons_YoungerThan50() =>
        FindPagedMustFilterRecords(qb => qb.Where(p => p.Age).IsSmallerThan(50), p => p.Age < 50);

    [Fact]
    public void Must_Filter_Persons_Named_Mani() =>
        FindPagedMustFilterRecords(qb => qb.Where(p => p.Name).IsEqualTo("Mani"), p => p.Name == "Mani");

    [Fact]
    public void Must_Filter_Persons_Named_Mina_OR_Farshid() =>
        FindPagedMustFilterRecords(qb => qb.Where(p => p.Name).IsEqualTo("Mina", "Farshid"),
            p => p.Name == "Mina" || p.Name == "Farshid");


    [Fact]
    public void Must_Filter_Persons_NOT_Named_Mina_NOR_Farshid() =>
        FindPagedMustFilterRecords(qb => qb.Where(p => p.Name).IsNotEqualTo("Mina", "Farshid"),
            p => p.Name != "Mina" && p.Name != "Farshid");


    [Fact]
    public void Must_Paginate_Any_Combination_AsExpected()
    {
        var env = new PersonsEnvironment();

        env.Perform(databases, e =>
        {
            var all = e.FindPaged<Person>().FromStorage;

            AssertX.ContainSameItems(e.GetPersons(p => true).ToList(), all);

            for (int size = 1; size < all.Count; size++)
            {
                for (int offset = 0; offset < all.Count - 1; offset++)
                {
                    var expected = all.Skip(offset).Take(size).ToList();

                    var actual = e.FindPaged<Person>(offset: offset, size: size).FromStorage;

                    AssertX.ContainSameItems(expected, actual);
                }
            }
        });
    }

    [Fact]
    public void Must_Sort_By_Age_Ascending()
        => FindPagedMustPerformCorrectSorting((p1, p2) => p1.Age - p2.Age,
            o => o.OrderAscendingBy(p => p.Age));

    [Fact]
    public void Must_Sort_By_Age_Descending()
        => FindPagedMustPerformCorrectSorting((p1, p2) => p2.Age - p1.Age,
            o => o.OrderDescendingBy(p => p.Age));


    [Fact]
    public void Must_Sort_By_Name_Ascending()
        => FindPagedMustPerformCorrectSorting((p1, p2) => string.Compare(p1.Name, p2.Name, StringComparison.Ordinal),
            o => o.OrderAscendingBy(p => p.Name));

    [Fact]
    public void Must_Sort_By_Name_Descending()
        => FindPagedMustPerformCorrectSorting((p1, p2) => string.Compare(p2.Name, p1.Name, StringComparison.Ordinal),
            o => o.OrderDescendingBy(p => p.Name));


    [Fact]
    public void Must_Find_ByPortionOf_SurName()
        => FindPagedMustFindExpectedItemsForGivenSearchTerms(p =>
            p.Name == "Mani" ||
            p.Name == "Mona" ||
            p.Name == "Farshid", "Moai");

    [Fact]
    public void Must_Find_ByPortionOf_Name()
        => FindPagedMustFindExpectedItemsForGivenSearchTerms(p =>
            p.Name == "Farshid" ||
            p.Name == "Farimehr", "far");

    [Fact]
    public void Must_Find_Name_IgnoreCase()
        => FindPagedMustFindExpectedItemsForGivenSearchTerms(p =>
            p.Name == "Farshid", "farshid");


    [Theory]
    [InlineData("Mina")]
    [InlineData("Mani")]
    [InlineData("Mona")]
    public void Must_Find_Only_UnDeletedResults(string deleteeName)
    {
        var env = new PersonsEnvironment();

        env.Perform(databases, e =>
        {
            e.Update<Person>(p => p.Name == deleteeName, p => p.IsDeleted = true);

            var expectedResult = e.GetPersons(p => p.Name != deleteeName).ToList();

            var found = e.FindPaged<Person>().FromStorage;

            AssertX.ContainSameItems(expectedResult, found, personIdentifier);
        });
    }


    private void FindPagedMustFindExpectedItemsForGivenSearchTerms(Func<Person, bool> predicate, params string[] searchTerms)
    {
        var env = new PersonsEnvironment();

        env.Perform(databases, e =>
        {
            e.Index(e.GetPersons(p => true));

            var terms = e.Transliterate(searchTerms);

            var expectedResult = e.GetPersons(predicate).ToList();

            var found = e.FindPaged<Person>(searchTerms: terms).FromStorage;

            AssertX.ContainSameItems(expectedResult, found, personIdentifier);
        });
    }


    private void FindPagedMustFilterRecords(Action<FilterQueryBuilder<Person>> qb, Func<Person, bool> predicate)
    {
        var env = new PersonsEnvironment();

        env.Perform(databases, e =>
        {
            var found = e.FindPaged(qb).FromStorage;

            AssertX.ContainSameItems(e.GetPersons(predicate).ToList(), found, personIdentifier);
        });
    }

    private void FindPagedMustPerformCorrectSorting(Comparison<Person> expectedComparer, Action<OrderSetBuilder<Person>> buildActualOrders)
    {
        var env = new PersonsEnvironment();

        env.Perform(databases, e =>
        {
            var found = e.FindPaged(order: buildActualOrders).FromStorage;

            var expected = e.GetSorted(expectedComparer);

            AssertX.AreSameSize(expected, found, "Read items does not match with expectations");

            AssertX.InSameOrder(expected, found);
        });
    }
}