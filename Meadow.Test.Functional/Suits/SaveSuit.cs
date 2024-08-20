using System.Collections.Generic;
using System.Linq;
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
        
        environment.Perform(Databases.SqLite,new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            expectedResult = c.Data.Get<Person>(p => p.Name == "Mani").First();

            expectedResult.Id = 1000; // Make sure id would not find it
            expectedResult.JobId = 1000; // Make sure job id would not find it 
            
            savedPersons = c.Save<Person>(p => p.Name == "Mani", m =>
            {
                m.Age = 1234;
                
            },"FullName");
        });

        Assert.Single(savedPersons);

        AssertX.AreEqualShallow(expectedResult, savedPersons.First());
    }
    
    [Fact]
    public void Should_Update_Existing_OnlyFound_By_FamilyJob_Collection()
    {
        var environment = new Environment<PersonsDataProvider>();

        var savedPersons = new List<Person>();
        var expectedResult = new Person();
        
        environment.Perform(Databases.SqLite,new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            expectedResult = c.Data.Get<Person>(p => p.Name == "Mani").First();
            expectedResult.Id = 1000; // Make sure id would not find it
            expectedResult.Name = "1000"; // Make sure name would not find it 
            
            savedPersons = c.Save<Person>(p => p.Id == expectedResult.Id, m =>
            {
                m.Age = 1234;
                
            },"FamilyJob");
        });

        Assert.Single(savedPersons);

        AssertX.AreEqualShallow(expectedResult, savedPersons.First());
    }
}