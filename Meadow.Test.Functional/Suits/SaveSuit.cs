using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Meadow.Scaffolding.Models;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

public class SaveSuit
{

    private readonly ITestOutputHelper _outputHelper;
    private readonly Func<Person, string> _toString = person => $"{person.Name}:{person.Id}";
    
    public SaveSuit(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }


    [Fact]
    public void Should_Update_Existing_OnlyFound_By_FullName_Collection()
    {
        var environment = new Environment<PersonsDataProvider>();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();
        
        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;
        
        environment.Perform(Databases.SqLite,new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;
            
            expectedResult = c.Data.Get<Person>(p => p.Name == "Mani").First();

            expectedResult.Id = 1000; // Make sure id would not find it
            expectedResult.JobId = 1000; // Make sure job id would not find it 
            
            savedPersons = c.Save<Person>(p => p.Name == "Mani", m =>
            {
                m.Age = 1234;
                
            },"FullName");
            
            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(expectedResult, savedPersons.First(),toString:_toString);
        
        Assert.Equal(existingItemsCount,afterSaveItemsCount);
    }
    
    [Fact]
    public void Should_Update_Existing_OnlyFound_By_FamilyJob_Collection()
    {
        var environment = new Environment<PersonsDataProvider>();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();
        
        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;
        
        environment.Perform(Databases.SqLite,new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;
            
            expectedResult = c.Data.Get<Person>(p => p.Name == "Mani").First();
            expectedResult.Id = 1000; // Make sure id would not find it
            expectedResult.Name = "1000"; // Make sure name would not find it 
            
            savedPersons = c.Save<Person>(p => p.Id == expectedResult.Id, m =>
            {
                m.Age = 1234;
                
            },"FamilyJob");
            
            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(expectedResult, savedPersons.First(),toString:_toString);
        
        Assert.Equal(existingItemsCount,afterSaveItemsCount);
    }
    
    [Fact]
    public void Should_Update_Existing_OnlyFound_By_Id_Collection()
    {
        var environment = new Environment<PersonsDataProvider>();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();
        
        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;
        
        environment.Perform(Databases.SqLite,new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;
            
            expectedResult = c.Data.Get<Person>(p => p.Name == "Mani").First();
            
            savedPersons = c.Save<Person>(p => p.Id == expectedResult.Id, m =>
            {
                m.Age = 1234;
                
            },CollectiveIdentificationProfile.IdCollectionName);
            
            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(expectedResult, savedPersons.First(),toString:_toString);
        
        Assert.Equal(existingItemsCount,afterSaveItemsCount);
    }
    
    [Fact]
    public void Should_Update_Existing_Found_By_Default_Collection()
    {
        var environment = new Environment<PersonsDataProvider>();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();
        
        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;
        
        environment.Perform(Databases.SqLite,new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;
            
            expectedResult = c.Data.Get<Person>(p => true).First();
            
            savedPersons = c.Save<Person>(p => p.Id == expectedResult.Id, m =>
            {
                m.Age = 1234;
                
            });
            
            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(expectedResult, savedPersons.First(),toString:_toString);
        
        Assert.Equal(existingItemsCount,afterSaveItemsCount);
    }
    
    [Fact]
    public void Should_CreateNew_NotFound_By_Default_Collection()
    {
        var environment = new Environment<PersonsDataProvider>();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();

        int existingItemsCount = 0;
        int afterSaveItemsCount = 0;
        
        environment.Perform(Databases.SqLite,new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            existingItemsCount = c.Data.Get<Person>().Count;
            
            expectedResult = c.Data.Get<Person>(p => true).First();

            expectedResult.JobId = 10000;
            
            savedPersons = c.Save<Person>(p => p.Id == expectedResult.Id, m =>
            {
                m.Age = 1234;
                
            });

            afterSaveItemsCount = c.FindPaged<Person>().FromStorage.Count;
        });

        Assert.Single(savedPersons);

        AssertX.AreEqual(expectedResult, savedPersons.First(),toString:_toString);
        
        Assert.Equal(existingItemsCount+1,afterSaveItemsCount);
    }
}