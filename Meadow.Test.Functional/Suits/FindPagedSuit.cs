using System;
using System.Linq;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Xunit;

namespace Meadow.Test.Functional.Suits;

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class FindPagedSuit
{
    private const Databases Databases = TestEnvironment.Databases.SqLite;

    private readonly Func<Person, string> _personIdentifier = p => $"{p.Name}:{p.Id}";


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Filter_Persons_OlderThan50(bool fullTree) =>
        FindPagedMustFilterRecords(qb => qb.Where(p => p.Age).IsLargerThan(50), p => p.Age > 50, fullTree);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Filter_Persons_YoungerThan50(bool fullTree) =>
        FindPagedMustFilterRecords(qb => qb.Where(p => p.Age).IsSmallerThan(50), p => p.Age < 50, fullTree);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Filter_Persons_Named_Mani(bool fullTree) =>
        FindPagedMustFilterRecords(qb => qb.Where(p => p.Name).IsEqualTo("Mani"), p => p.Name == "Mani", fullTree);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Filter_Persons_Named_Mina_OR_Farshid(bool fullTree) =>
        FindPagedMustFilterRecords(qb => qb.Where(p => p.Name).IsEqualTo("Mina", "Farshid"),
            p => p.Name == "Mina" || p.Name == "Farshid", fullTree);


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Filter_Persons_NOT_Named_Mina_NOR_Farshid(bool fullTree) =>
        FindPagedMustFilterRecords(qb => qb.Where(p => p.Name).IsNotEqualTo("Mina", "Farshid"),
            p => p.Name != "Mina" && p.Name != "Farshid", fullTree);


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Paginate_Any_Combination_AsExpected(bool fullTree)
    {
        var env = new Environment<PersonsDataProvider>();

        env.Perform(Databases, e =>
        {
            var all = e.FindPaged<Person>(fullTree: fullTree).FromStorage;

            AssertX.ContainSameItems(e.Data.Get<Person>(p => true).ToList(),
                all, _personIdentifier, true, fullTree);

            for (int size = 1; size < all.Count; size++)
            {
                for (int offset = 0; offset < all.Count - 1; offset++)
                {
                    var expected = all.Skip(offset).Take(size).ToList();

                    var actual = e.FindPaged<Person>(offset: offset, size: size, fullTree: fullTree).FromStorage;

                    AssertX.ContainSameItems(expected, actual, _personIdentifier, true, fullTree);
                }
            }
        });
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Sort_By_Age_Ascending(bool fullTree)
        => FindPagedMustPerformCorrectSorting((p1, p2) => p1.Age - p2.Age,
            o => o.OrderAscendingBy(p => p.Age), fullTree);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Sort_By_Age_Descending(bool fullTree)
        => FindPagedMustPerformCorrectSorting((p1, p2) => p2.Age - p1.Age,
            o => o.OrderDescendingBy(p => p.Age), fullTree);


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Sort_By_Name_Ascending(bool fullTree)
        => FindPagedMustPerformCorrectSorting((p1, p2) => string.Compare(p1.Name, p2.Name, StringComparison.Ordinal),
            o => o.OrderAscendingBy(p => p.Name), fullTree);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Sort_By_Name_Descending(bool fullTree)
        => FindPagedMustPerformCorrectSorting((p1, p2) => string.Compare(p2.Name, p1.Name, StringComparison.Ordinal),
            o => o.OrderDescendingBy(p => p.Name), fullTree);


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Find_ByPortionOf_SurName(bool fullTree)
        => FindPagedMustFindExpectedItemsForGivenSearchTerms(p =>
            p.Name == "Mani" ||
            p.Name == "Mona" ||
            p.Name == "Farshid", fullTree, "Moai");

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Find_ByPortionOf_Name(bool fullTree)
        => FindPagedMustFindExpectedItemsForGivenSearchTerms(p =>
            p.Name == "Farshid" ||
            p.Name == "Farimehr", fullTree, "far");

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Must_Find_Name_IgnoreCase(bool fullTree)
        => FindPagedMustFindExpectedItemsForGivenSearchTerms(p =>
            p.Name == "Farshid", fullTree, "farshid");


    [Theory]
    [InlineData("Mina", false)]
    [InlineData("Mina", true)]
    [InlineData("Mani", false)]
    [InlineData("Mani", true)]
    [InlineData("Mona", false)]
    [InlineData("Mona", true)]
    public void Must_Find_Only_UnDeletedResults(string deleteeName, bool fullTree)
    {
        var env = new Environment<PersonsDataProvider>();

        env.RegulateMeadowConfigurations(configurations => { configurations.AddFilter<Person>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false)); });


        env.Perform(Databases, e =>
        {
            e.Update<Person>(p => p.Name == deleteeName, p => p.IsDeleted = true);

            var expectedResult = e.Data.Get<Person>(p => p.Name != deleteeName).ToList();

            var found = e.FindPaged<Person>(fullTree:fullTree).FromStorage;

            AssertX.ContainSameItems(expectedResult, found, _personIdentifier,true,fullTree);
        });
    }


    private void FindPagedMustFindExpectedItemsForGivenSearchTerms(Func<Person, bool> predicate, bool fullTree, params string[] searchTerms)
    {
        var env = new Environment<PersonsDataProvider>();

        env.Perform(Databases, e =>
        {
            e.Index(e.Data.Get<Person>(p => true));
            
            var expectedResult = e.Data.Get(predicate).ToList();

            var found = e.FindPaged<Person>(searchTerms: searchTerms, fullTree: fullTree).FromStorage;

            AssertX.ContainSameItems(expectedResult, found, _personIdentifier, true, fullTree);
        });
    }


    private void FindPagedMustFilterRecords(Action<FilterQueryBuilder<Person>> qb, Func<Person, bool> predicate, bool fullTree)
    {
        var env = new Environment<PersonsDataProvider>();

        env.Perform(Databases, e =>
        {
            var found = e.FindPaged(qb, fullTree: fullTree).FromStorage;

            AssertX.ContainSameItems(expectedItems: e.Data.Get(predicate).ToList(),
                actualItems: found,
                toString: _personIdentifier,
                ignoreId: true,
                deepCompare: fullTree);
        });
    }

    private void FindPagedMustPerformCorrectSorting(Comparison<Person> expectedComparer, Action<OrderSetBuilder<Person>> buildActualOrders, bool fullTree)
    {
        var env = new Environment<PersonsDataProvider>();

        env.Perform(Databases, e =>
        {
            var found = e.FindPaged(order: buildActualOrders, fullTree: fullTree).FromStorage;

            var expected = e.Data.Get(expectedComparer);

            AssertX.AreSameSize(expected, found, "Read items does not match with expectations");

            AssertX.AreInSameOrder(expected, found, _personIdentifier, true, fullTree);
        });
    }
}