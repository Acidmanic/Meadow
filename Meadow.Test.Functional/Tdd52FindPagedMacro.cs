using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional;

public class Tdd52FindPagedMacro : PersonUseCaseTestBase
{
    protected override void SelectDatabase()
    {
        UseSqLite();
    }


    protected override MeadowConfiguration RegulateConfigurations(MeadowConfiguration configurations)
    {
        configurations.AddFilter<Deletable>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
        configurations.AddFilter<Person>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
        configurations.AddFilter<Job>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
        configurations.AddFilter<Address>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));


        return configurations;
    }

    
    protected override void Main(MeadowEngine engine, ILogger logger)
    {
        var allPersonResponse = engine
            .PerformRequest(new FindPagedRequest<Person, long>(new FilterQuery()));

        if (allPersonResponse.Failed)
        {
            throw allPersonResponse.FailureException;
        }

        var allPersons = allPersonResponse.FromStorage;

        if (allPersons.Count != Persons.Length)
        {
            throw new Exception("Problem with reading all records in find pages");
        }
    }
}