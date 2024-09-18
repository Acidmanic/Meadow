using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Meadow.Test.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class DataBoundSuit
{

    private const Databases Database = Databases.MySql;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _scriptsDirectory = "SnippetComposedMacroScripts";

    public DataBoundSuit(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    [Fact]
    public void Should_Create_Environment_Without_Exception()
    {
        var environment = CreateEnvironment();

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            
            
        });
    }


    [Fact]
    public void Should_Return_CorrectRange_ForNames() => Should_Return_CorrectRange(
        p => p.Name,
        EntityBoundDataProvider.MinimumName,
        EntityBoundDataProvider.MaximumName);
    
    [Fact]
    public void Should_Return_CorrectRange_ForSurnames() => Should_Return_CorrectRange(
        p => p.Surname,
        EntityBoundDataProvider.MinimumSurname,
        EntityBoundDataProvider.MaximumSurname);
    
    [Fact]
    public void Should_Return_CorrectRange_ForAges() => Should_Return_CorrectRange(
        p => p.Age,
        EntityBoundDataProvider.MinimumAge,
        EntityBoundDataProvider.MaximumAge);
    
    
    private void Should_Return_CorrectRange<TField>(Expression<Func<Person,TField>> field, TField min,TField max)
    {
        var environment = CreateEnvironment();

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            var range = c.Range(field);

            Assert.NotNull(range);
            
            c.Logger.LogInformation("Range For {Field} -> Min: {Min}, Max: {Max}",MemberOwnerUtilities.GetAddress(field),range.Min,range.Max);
            
            Assert.Equal(min, range.Min);
            
            Assert.Equal(max, range.Max);
        });
    }

    [Fact]
    public void Should_Return_CorrectExistingValues_For_Name() => 
        Should_Return_CorrectExistingValues(p => p.Name, EntityBoundDataProvider.ExistingNames);
    
    [Fact]
    public void Should_Return_CorrectExistingValues_For_Surname() => 
        Should_Return_CorrectExistingValues(p => p.Surname, EntityBoundDataProvider.ExistingSurnames);
    
    [Fact]
    public void Should_Return_CorrectExistingValues_For_Ages() => 
        Should_Return_CorrectExistingValues(p => p.Age, EntityBoundDataProvider.ExistingAges);
    
    private void Should_Return_CorrectExistingValues<TField>(Expression<Func<Person,TField>> field, TField[] expectedExistingValues)
    {
        var environment = CreateEnvironment();

        environment.Perform(Database, new LoggerAdapter(_testOutputHelper.WriteLine), c =>
        {
            var actualExistings = c.Existings(field);
            
            AssertX.ContainSameItems(expectedExistingValues.ToList(),actualExistings);
            
            c.Logger.LogInformation("Range For {Values}",string.Join(',',expectedExistingValues));
            
        });
    }


    private Environment<EntityBoundDataProvider> CreateEnvironment()
    {
        var environment = new Environment<EntityBoundDataProvider>(_scriptsDirectory,GetType().Name);

        return environment;
    }
}