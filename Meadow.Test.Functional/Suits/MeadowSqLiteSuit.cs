using System;
using System.IO;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Meadow.Test.Functional.TestEnvironment.Utility;
using Meadow.Test.Shared;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.LightWeight;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Meadow.Test.Functional.Suits;

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class MeadowSqLiteSuit
{
    private readonly ITestOutputHelper _helper;

    public MeadowSqLiteSuit(ITestOutputHelper helper)
    {
        _helper = helper;
    }

    [Fact]
    public void SqLite_ShouldBeAbleTo_Create_Successively()
    {
        for (int i = 0; i < 5; i++)
        {
            var setup = new MeadowEngineSetup();

            setup.SelectDatabase(Databases.SqLite);

            var engine = setup.CreateEngine();

            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }

            engine.CreateDatabase();

            engine.BuildUpDatabase();

            _helper.WriteLine("Round: {0} Performed Successfully", i);
        }
    }
    
    [Fact]
    public void DropDatabase_ShouldLeaveNoFile()
    {
        for (int i = 0; i < 5; i++)
        {
            var setup = new MeadowEngineSetup();

            MeadowEngine.UseLogger(new LoggerAdapter(_helper.WriteLine));
            
            setup.SelectDatabase(Databases.SqLite);

            var engine = setup.CreateEngine();

            if (engine.DatabaseExists())
            {
                _helper.WriteLine("Dropping EXISTING Database");
                
                engine.DropDatabase();
                
            }
            else
            {
                _helper.WriteLine("There was No Database already.");
            }

            AssertDatabaseFileExistence(false,setup.DatabaseName);
            AssertDatabaseFileExistence(false,setup.DatabaseName,true);
            
            engine.CreateDatabase();

            _helper.WriteLine("Database Created");
            
            LogExistence(setup.DatabaseName);
            
            engine.BuildUpDatabase();
            
            AssertDatabaseFileExistence(true,setup.DatabaseName);
            AssertDatabaseFileExistence(true,setup.DatabaseName,true);

            _helper.WriteLine("Round: {0} Performed Successfully", i);
            
            MeadowEngine.UseLogger(NullLogger.Instance);
        }
    }
    
    [Fact]
    public void SqLite_ShouldBeAbleTo_CallProcedures_Successively()
    {
        for (int i = 0; i < 5; i++)
        {
            _helper.WriteLine("Starting Round: {0}...", i);
            
            var environment = new Environment<PersonsDataProvider>();
            
            environment.Perform(Databases.SqLite,new LoggerAdapter(_helper.WriteLine), e =>
            {
                var persons = e.FindPaged<Person>();
                
                _helper.WriteLine("Round: {0} Read {1} Persons",i,persons.FromStorage.Count);
            });

            _helper.WriteLine("Round: {0} Performed Successfully", i);
        }
    }
    
    
    private void AssertDatabaseFileExistence(bool exists, string databaseName, bool json = false)
    {
        var directory = new FileInfo(typeof(MeadowSqLiteSuit).Assembly.Location).Directory?.FullName ?? 
                        new DirectoryInfo(".").FullName;

        var dbFileName = databaseName + ".db";
        
        if (json) dbFileName += ".json";
        
        var file = Path.Combine(directory, dbFileName);

        var actualExists = File.Exists(file);

        var fileTitle = json ? "Procedures" : "Data";

        var not = exists ? "" : " NOT";
        var but = exists ? "does not" : "does";
        if (exists != actualExists)
        {
            throw new XunitException($"Expected {fileTitle} file to {not} Exist, but it {but}:" +
                                     $"\n{file}");
        }
        _helper.WriteLine("[PASS] Expectation:{0} {1} to exist.",fileTitle,not);
    }
    
    private void LogExistence(string databaseName)
    {
        var directory = new FileInfo(typeof(MeadowSqLiteSuit).Assembly.Location).Directory?.FullName ?? 
                        new DirectoryInfo(".").FullName;

        var dataFileName = databaseName + ".db";
        var proceduresFileName = databaseName + ".db.json";
        
        
        var dataFilePath = Path.Combine(directory, dataFileName);
        var proceduresFilePath = Path.Combine(directory, proceduresFileName);

        var dataExists = File.Exists(dataFilePath);
        var proceduresExists = File.Exists(proceduresFilePath);

        _helper.WriteLine("[Data]:       {0}",dataExists?" EXISTS":"does NOT exist");
        _helper.WriteLine("[Procedures]: {0}",proceduresExists?" EXISTS":"does NOT exist");
    }
}