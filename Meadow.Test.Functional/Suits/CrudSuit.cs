using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class CrudSuit
{
    private const Databases Databases = TestEnvironment.Databases.SqLite;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Func<Person, string> _personIdentifier = p => $"{p.Name}:{p.Id}";

    public CrudSuit(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(false,false)]
    [InlineData(false,true)]
    [InlineData(true,false)]
    [InlineData(true,true)]
    public void ShouldBeAble_ToRead_All(bool fullTree, bool considerEntityFilters)
    {
        var environment = new Environment<PersonsDataProvider>();

        if (considerEntityFilters)
        {
            environment.RegulateMeadowConfigurations(c =>
            {
                c.AddFilter<Person>(builder => builder.Where(p => p.IsDeleted).IsEqualTo(false));
            });
        }

        var actual = new List<Person>();
        
        var expected = new List<Person>();
        
        environment.Perform(Databases,new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            expected = c.Data.Get<Person>(p => (!considerEntityFilters) || p.IsDeleted == false);

            actual = c.ReadAll<Person>(fullTree).FromStorage;
        });
        
        AssertX.ContainSameItems(expected,actual, _personIdentifier,true,fullTree);
    }
    
    
    
    

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldBeAble_ToRead_ById(bool fullTree)
    {
        var environment = new Environment<PersonsDataProvider>();

        environment.RegulateMeadowConfigurations(c => { c.AddFilter<Person>(builder => builder.Where(p => p.IsDeleted).IsEqualTo(false)); });

        var actuals = new List<Person>();
        List<Person> expecteds = new List<Person>();

        environment.Perform(Databases, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            expecteds = c.Data.Get<Person>(p => !p.IsDeleted);

            foreach (var expected in expecteds)
            {
                var actual = c.ReadById<Person, long>(expected.Id, fullTree).FromStorage.FirstOrDefault();

                if (actual is { } a) actuals.Add(a);
            }
        });

        AssertX.ContainSameItems(expecteds, actuals, _personIdentifier, true, fullTree);
    }


    [Theory]
    [InlineData("Mani")]
    [InlineData("Mona")]
    [InlineData("Mina")]
    [InlineData("Farshid")]
    public void Deleted_Items_Should_NOT_Be_ReadAgain(string deletee)
    {
        var environment = new Environment<PersonsDataProvider>();

        Person? actualDeleted = null;
        var actualUnDeleted = new List<Person>();
        var expectedUndeleted = new List<Person>();

        environment.Perform(Databases, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {

            expectedUndeleted = c.Data.Get<Person>(p => p.Name != deletee);
            
            var deleteId = c.Data.Get<Person>(p => p.Name == deletee).Single().Id;

            c.DeleteById<Person, long>(deleteId);

            actualDeleted = c.ReadById<Person, long>(deleteId).FromStorage.FirstOrDefault();

            actualUnDeleted = c.ReadAll<Person>().FromStorage;
        });

        Assert.Null(actualDeleted);

        AssertX.ContainSameItems(expectedUndeleted, actualUnDeleted,_personIdentifier,true,false);
    }
}