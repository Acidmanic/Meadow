using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Meadow.Test.Functional.TestEnvironment.Extensions;
using Meadow.Test.Shared;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class DirectRequestsSuit
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly Func<Person, string> _toString = person => $"{person.Name}:{person.Id}";
    private readonly Databases _database = Databases.MySql;
    public DirectRequestsSuit(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }


    [Fact]
    public void Should_Insert_WithDirectInsert()
    {
        var environment = CreateEnvironment();
        
        environment.Perform(_database, new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            var model = new Person()
            {
                Age = 1234,
                Name = "Inserted",
                Surname = "Directly",
                JobId = 4321
            };
            
            c.DirectPerform(c.TranslateInsert(model));
        });

        
    }
    
    [Fact]
    public void Should_Insert_WithDirectInsert_Event()
    {
        var environment = new Environment<StatisticsDataProvider>();
        
        environment.Perform(_database, new LoggerAdapter(_outputHelper.WriteLine), c =>
        {
            var model = new Person()
            {
                Age = 1234,
                Name = "Inserted",
                Surname = "Directly",
                JobId = 4321
            };
            
            c.DirectPerform(c.TranslateInsert(model));
        });

        
    }


    private Environment<PersonsDataProvider> CreateEnvironment()
    {
        var environment = new Environment<PersonsDataProvider>();
        
        return environment;
    }
}