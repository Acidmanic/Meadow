using System;
using Meadow.Configuration;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;

namespace Meadow.Test.Functional;

public class Tdd51EntityFilters:MeadowMultiDatabaseTestBase
{
    protected override void SelectDatabase()
    {
        UseSqLite();
    }


    protected override MeadowConfiguration RegulateConfigurations(MeadowConfiguration configurations)
    {

        configurations.AddFilter<Deletable>(builder => builder.Where(d => d.IsDeleted).IsEqualTo("0"));


        return configurations;
    }

    protected override void Main(MeadowEngine engine, ILogger logger)
    {

        var data = new Deletable[]
        {
            new Deletable(){Id = 1},
            new Deletable(){Id = 2},
            new Deletable(){Id = 3},
            new Deletable(){Id = 4},
        };
        
        InsertAll(engine, data);


        var all = engine.PerformRequest(new ReadAllRequest<Deletable>()).FromStorage;

        if (all.Count != data.Length)
        {
            throw new Exception("Expected to retrieve all seed data, but it did not");
        }

        var sample1 = data[1];

        sample1.IsDeleted = true;

        var updated = engine.PerformRequest(new UpdateRequest<Deletable>(sample1));
        

        var allAfterSoftDelete = engine.PerformRequest(new ReadAllRequest<Deletable>()).FromStorage;
        
        
        if (allAfterSoftDelete.Count != data.Length -1)
        {
            throw new Exception("Expected to retrieve Only UnDeleted Data, but it did not");
        }
        
    }
}