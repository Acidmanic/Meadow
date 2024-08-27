using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class SnippetComposedSaveSuit
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly Func<Person, string> _toString = person => $"{person.Name}:{person.Id}";
    private readonly Databases _database = Databases.SqLite;
    public SnippetComposedSaveSuit(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }


    [Fact]
    public void Should_Update_Existing_OnlyFound_By_FullName_Collection()
    {
        var environment = CreateEnvironment();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();

        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;

        environment.Perform(_database, new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;

            expectedResult = c.Data.Get<Person>(p => p.Name == "Mani").First();

            expectedResult.Id = 1000; // Make sure id would not find it
            expectedResult.JobId = 1000; // Make sure job id would not find it 

            savedPersons = c.Save<Person>(p => p.Name == "Mani", m => { m.Age = 1234; }, "FullName");

            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(expectedResult, savedPersons.First(), toString: _toString);

        Assert.Equal(existingItemsCount, afterSaveItemsCount);
    }

    [Fact]
    public void Should_Update_Existing_OnlyFound_By_FamilyJob_Collection()
    {
        var environment = CreateEnvironment();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();

        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;

        environment.Perform(_database, new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;

            expectedResult = c.Data.Get<Person>(p => p.Name == "Mani").First();
            expectedResult.Id = 1000; // Make sure id would not find it
            expectedResult.Name = "1000"; // Make sure name would not find it 

            savedPersons = c.Save<Person>(p => p.Id == expectedResult.Id, m => { m.Age = 1234; }, "FamilyJob");

            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(expectedResult, savedPersons.First(), toString: _toString);

        Assert.Equal(existingItemsCount, afterSaveItemsCount);
    }

    [Fact]
    public void Should_Update_Existing_OnlyFound_By_Id_Collection()
    {
        var environment = CreateEnvironment();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();

        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;

        environment.Perform(_database, new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;

            expectedResult = c.Data.Get<Person>(p => p.Name == "Mani").First();

            savedPersons = c.Save<Person>(p => p.Id == expectedResult.Id, m => { m.Age = 1234; }, nameof(Person.Id));

            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(expectedResult, savedPersons.First(), toString: _toString);

        Assert.Equal(existingItemsCount, afterSaveItemsCount);
    }

    [Fact]
    public void Should_Update_Existing_Found_By_Values()
    {
        var environment = CreateEnvironment();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();

        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;

        environment.Perform(_database, new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;

            expectedResult = c.Data.Get<Person>(p => true).First();

            savedPersons = c.Save<Person>(p => p.Id == expectedResult.Id, m => { });

            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(expectedResult, savedPersons.First(), toString: _toString);

        Assert.Equal(existingItemsCount, afterSaveItemsCount);
    }

    [Fact]
    public void Should_CreateNew_NotFound_By_Default_Collection()
    {
        var environment = CreateEnvironment();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();

        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;

        environment.Perform(_database, new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;

            expectedResult = c.Data.Get<Person>(p => true).First();

            savedPersons = c.Save<Person>(p => p.Id == expectedResult.Id, m => { m.Age = 1234; });

            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(expectedResult, savedPersons.First(), toString: _toString);

        Assert.Equal(existingItemsCount + 1, afterSaveItemsCount);
    }

    [Fact]
    public void Should_CreateNew_When_Removed_By_EntityFilters()
    {
        var environment = CreateEnvironment();

        var savedPersons = new List<Person>();
        var itemOfInterest = new Person();

        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;

        environment.Perform(_database, new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;

            itemOfInterest = c.Data.Get<Person>(p => p.IsDeleted).First();

            savedPersons = c.Save<Person>(p => p.Id == itemOfInterest.Id, m => { m.Age = 1234; });

            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(itemOfInterest, savedPersons.First(), toString: _toString);

        Assert.Equal(existingItemsCount + 1, afterSaveItemsCount);
    }


    [Fact]
    public void Should_CreateNewItem_WhenNewValueIntroduces()
    {
        var environment = CreateEnvironment();

        var savedItem = new Tag();
        var item = new Tag() { PropertyId = 100, ProductClassId = 200 };

        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;

        environment.Perform(_database, new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Tag>().Count;

            savedItem = c.Save(item);

            afterSaveItemsCount = c.ReadAll<Tag>().FromStorage.Count;
        });

        Assert.NotNull(savedItem);

        AssertX.AreEqual(item, savedItem, t => $"{t.ProductClassId}:{t.PropertyId}");

        Assert.Equal(existingItemsCount + 1, afterSaveItemsCount);
    }


    private Environment<PersonsDataProvider> CreateEnvironment()
    {
        var environment = new Environment<PersonsDataProvider>();

        environment.RegulateMeadowConfigurations(config =>
        {
            var file = Path.Combine(config.BuildupScriptDirectory, "0003-Person.sql");

            File.Delete(file);

            var content = "-- {{WipAll Meadow.Test.Functional.Models.Person}}";
            File.WriteAllText(file, content);

            

            file = Path.Combine(config.BuildupScriptDirectory, "0005-Tags.sql");

            File.Delete(file);
            
            content = "-- {{WipSave Meadow.Test.Functional.Models.Tag}}";
            
            File.WriteAllText(file, content);
        });

        return environment;
    }
}