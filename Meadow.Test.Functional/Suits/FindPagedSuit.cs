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

            AssertX.ContainSameItems(e.GetPersons(p=>true).ToList(), all);
            
            for (int size = 1; size < all.Count; size++)
            {
                for (int offset = 0; offset < all.Count-1; offset++)
                {

                    var expected = all.Skip(offset).Take(size).ToList();

                    var actual = e.FindPaged<Person>(offset: offset, size: size).FromStorage;
                    
                    AssertX.ContainSameItems(expected,actual);
                }
            }

        });
    }
    
    
    
    public void FindPagedMustFindExpectedItemsForGivenSearchTerms(List<Person> expectedResult, string[] searchTerms)
    {
        var env = new PersonsEnvironment();

        env.Perform(databases, e =>
        {
            var terms = e.Transliterate(searchTerms);

            var found = e.FindPaged<Person>(searchTerms: terms).FromStorage;

            AssertX.ContainSameItems(expectedResult, found, personIdentifier);
        });
    }
    
    
    
    
    
    
    private void FindPagedMustFilterRecords(Action<FilterQueryBuilder<Person>> qb,Func<Person,bool> predicate)
    {

        var env = new PersonsEnvironment();

        env.Perform(databases, e =>
        {
            var found = e.FindPaged(qb).FromStorage;
            
            AssertX.ContainSameItems(e.GetPersons(predicate).ToList(), found, personIdentifier);
        });
        
    }
}