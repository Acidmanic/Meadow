using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional;

public class Tdd51EntityFilters : PersonUseCaseTestBase
{
    protected override void SelectDatabase()
    {
        UsePostgre();
    }


    protected override MeadowConfiguration RegulateConfigurations(MeadowConfiguration configurations)
    {
        configurations.AddFilter<Deletable>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
        configurations.AddFilter<Person>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
        configurations.AddFilter<Job>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
        configurations.AddFilter<Address>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));


        return configurations;
    }

    private record ReadChunkResult<T>(List<T> Items, int Count, int Offset, int Size);

    private ReadChunkResult<Deletable> Filter(MeadowEngine engine, Action<FilterQueryBuilder<Deletable>> filterBuilder,
        bool fullTreeRead = false)
    {
        var builder = new FilterQueryBuilder<Deletable>();

        filterBuilder(builder);

        var filter = builder.Build();

        var filterResult = engine.PerformRequest(new PerformSearchIfNeededRequest<Deletable, int>
                (filter, null, new string[] { }, new OrderTerm[] { }))
            .FromStorage
            .FirstOrDefault();

        if (filterResult?.SearchId is null) throw new Exception("Problem Performing Search/Filter");

        var sequence = engine.PerformRequest(new ReadChunkRequest<Deletable>(filterResult.SearchId, 0, int.MaxValue))
            .FromStorage;

        return new ReadChunkResult<Deletable>(sequence, sequence.Count, 0, int.MaxValue);
    }

    protected override void Main(MeadowEngine engine, ILogger logger)
    {
        var data = new Deletable[]
        {
            new Deletable() { Id = 1, Information = "First" },
            new Deletable() { Id = 2, Information = "Second" },
            new Deletable() { Id = 3, Information = "Third" },
            new Deletable() { Id = 4, Information = "Fourth" },
        };

        InsertAll(engine, data);


        var all = engine.PerformRequest(new ReadAllRequest<Deletable>()).FromStorage;

        if (all.Count != data.Length)
        {
            throw new Exception("Expected to retrieve all seed data, but it did not");
        }

        var sample1 = data.First(d => d.Id == 1);

        sample1.IsDeleted = true;

        var updated = engine.PerformRequest(new UpdateRequest<Deletable>(sample1)).FromStorage.FirstOrDefault();

        if (updated is not null) throw new Exception("Expected Update crud code to apply filters but it did not.");

        sample1.IsDeleted = false;
        
        updated = engine.PerformRequest(new UpdateRequest<Deletable>(sample1)).FromStorage.FirstOrDefault();
        
        if (updated is not null) throw new Exception("Expected Update crud code to apply filters but it did not.");

        var reReadDeletedSample1 = engine.PerformRequest(new ReadByIdRequest<Deletable, int>(1), false)
            .FromStorage.FirstOrDefault();
        
        if (reReadDeletedSample1 is not null) throw new Exception("Records not matching with EntityFilters, should not be updated, But it did");
        
        logger.LogInformation("[PASS] Update Is Fine");

        var allAfterSoftDelete = engine.PerformRequest(new ReadAllRequest<Deletable>()).FromStorage;

        if (allAfterSoftDelete.Count != data.Length - 1)
        {
            throw new Exception("Expected to retrieve Only UnDeleted Data, but it did not");
        }

        logger.LogInformation("[PASS] ReadAll Is Fine");

        var allAfterSoftDeleteFullTree = engine.PerformRequest(new ReadAllRequest<Deletable>(), true).FromStorage;


        if (allAfterSoftDeleteFullTree.Count != data.Length - 1)
        {
            throw new Exception("Expected to retrieve Only UnDeleted Data, but it did not");
        }

        logger.LogInformation("[PASS] ReadAll FullTree Is Fine");

        // Read By Id Test

        foreach (var item in data)
        {
            if (item.Id != sample1.Id)
            {
                var readUndeleted = engine.PerformRequest(new ReadByIdRequest<Deletable, int>(item.Id))
                    .FromStorage.FirstOrDefault();

                if (readUndeleted is null || readUndeleted.Id != item.Id)
                {
                    throw new Exception("Expected to retrieve UnDeleted Data, but it did not");
                }

                readUndeleted = engine.PerformRequest(new ReadByIdRequest<Deletable, int>(item.Id), true)
                    .FromStorage.FirstOrDefault();

                if (readUndeleted is null || readUndeleted.Id != item.Id)
                {
                    throw new Exception("Expected to retrieve UnDeleted Data, but it did not");
                }
            }
            else
            {
                var readDeleted = engine.PerformRequest(new ReadByIdRequest<Deletable, int>(item.Id))
                    .FromStorage.FirstOrDefault();

                if (readDeleted is not null)
                {
                    throw new Exception("Expected Filter to work and soft delete the item but it did not.");
                }

                readDeleted = engine.PerformRequest(new ReadByIdRequest<Deletable, int>(item.Id), true)
                    .FromStorage.FirstOrDefault();

                if (readDeleted is not null)
                {
                    throw new Exception("Expected Filter to work and soft delete the item but it did not.");
                }
            }
        }

        logger.LogInformation("[PASS] ReadById Is Fine");
        logger.LogInformation("[PASS] ReadById-FullTree Is Fine");

        var normalInsertSample = new Deletable() { Id = 5 };

        var inserted = engine.PerformRequest(new InsertRequest<Deletable>(normalInsertSample))
            .FromStorage.FirstOrDefault();

        if (inserted is null) throw new Exception("Expected to received inserted data but it did not");

        var alreadyDeleted = new Deletable() { Id = 6, IsDeleted = true };

        inserted = engine.PerformRequest(new InsertRequest<Deletable>(alreadyDeleted))
            .FromStorage.FirstOrDefault();

        if (inserted is not null) throw new Exception("Expected Insert method to apply entity filters but it did not");

        logger.LogInformation("[PASS] Insert Is Fine");

        var sample3 = data.First(d => d.Id == 3);

        sample3.IsDeleted = true;

        var savedMustBeNull = engine.PerformRequest(new SaveRequest<Deletable>(sample3)).FromStorage.FirstOrDefault();

        if (savedMustBeNull is not null)
            throw new Exception("Existing object Soft-deleted and Saved, must be returned null but it was not.");

        alreadyDeleted.Id = 7;

        savedMustBeNull = engine.PerformRequest(new SaveRequest<Deletable>(alreadyDeleted)).FromStorage
            .FirstOrDefault();

        if (savedMustBeNull is not null)
            throw new Exception("New Soft-deleted Saved object must be returned null but it was not.");

        normalInsertSample.Id = 8;

        var mustBeSavedNormally = engine.PerformRequest(new SaveRequest<Deletable>(normalInsertSample)).FromStorage
            .FirstOrDefault();

        if (mustBeSavedNormally is null)
            throw new Exception("New Un-Deleted object must be saved and returned with value.");

        var sample2 = data.First(d => d.Id == 2);

        mustBeSavedNormally = engine.PerformRequest(new SaveRequest<Deletable>(sample2)).FromStorage.FirstOrDefault();

        if (mustBeSavedNormally is null)
            throw new Exception("Existing Un-Deleted object must be saved and returned with value.");

        
        sample1.IsDeleted = true;
        
        engine.PerformRequest(new UpdateRequest<Deletable>(sample1));
        
        sample1.IsDeleted = false;
        
        var saveRepeatedSample1Response = engine.PerformRequest(new SaveRequest<Deletable>(sample1));
        
        if (!saveRepeatedSample1Response.Failed || saveRepeatedSample1Response.FailureException == null) throw new Exception("RepeatedId Should not be able to be inserted");


        logger.LogInformation("[PASS] Save Is Fine");


        var foundFirst = Filter(engine, b =>
            b.Where(d => d.Information).IsEqualTo("First"), false);

        if (foundFirst.Count > 0 || foundFirst.Items.Count > 0)
            throw new Exception("Expected to NOT TO find First item but it was returned.");


        var foundSecondFull = Filter(engine, b =>
            b.Where(d => d.Information).IsEqualTo("Second"), false);

        if (foundSecondFull.Count != 1 || foundSecondFull.Items.Count != 1 ||
            foundSecondFull.Items.First().Information != "Second")
            throw new Exception("Expected to find Second item but it did not.");

        var foundFirstFullTree = Filter(engine, b =>
            b.Where(d => d.Information).IsEqualTo("First"), true);

        if (foundFirstFullTree.Count > 0 || foundFirstFullTree.Items.Count > 0)
            throw new Exception("Expected to NOT TO find First item but it was returned.");


        var foundSecondFullTree = Filter(engine, b =>
            b.Where(d => d.Information).IsEqualTo("Second"), true);

        if (foundSecondFullTree.Count != 1 || foundSecondFullTree.Items.Count != 1 ||
            foundSecondFullTree.Items.First().Information != "Second")
            throw new Exception("Expected to find Second item but it did not.");

        logger.LogInformation("[PASS] Search/Filter Is Fine");

        var allSeededPersons = engine.PerformRequest
                (new ReadAllRequest<Person>(), true)
            .FromStorage;

        if (allSeededPersons.Count != Persons.Length)
        {
            throw new Exception("There was an issue either in seeding or reading persons");
        }

        var mani = allSeededPersons.First(p => p.Name == "Mani");

        mani.IsDeleted = true;
        
        engine.PerformRequest(new UpdateRequest<Person>(mani), true);
        
        var personsButMani =engine.PerformRequest
                (new ReadAllRequest<Person>(), true)
            .FromStorage;

        if (personsButMani.Count != Persons.Length - 1 || personsButMani.Any(p=> p.Name=="Mani"))
        {
            throw new Exception("Delete did not work on top level");
        }

        var farimehr = personsButMani.First(p => p.Name == "Farimehr");

        var farimehr3RdAddress = farimehr.Addresses.First(a => a.Block == 3);

        farimehr3RdAddress.IsDeleted = true;
        
        engine.PerformRequest(new UpdateRequest<Address>(farimehr3RdAddress), true);

        var reReadFarimehr = engine.PerformRequest(new ReadByIdRequest<Person, long>(farimehr.Id),true)
            .FromStorage.First();

        if (farimehr.Addresses.Count != reReadFarimehr.Addresses.Count + 1 ||
            reReadFarimehr.Addresses.Any(a => a.Block == 3))
        {
            throw new Exception("Entity filter (soft delete case) did not work for sub entities");
        }
        
        /*  Deleted children causes undeleted parent not be read case:   */
        
        reReadFarimehr.Addresses.ForEach( a =>
        {
            a.IsDeleted = true;

            engine.PerformRequest(new UpdateRequest<Address>(a));
        });
        
        reReadFarimehr = engine.PerformRequest(new ReadByIdRequest<Person, long>(farimehr.Id),true)
            .FromStorage.FirstOrDefault();


        if (reReadFarimehr == null)
        {
            throw new Exception("Top entity was not read when it's sub entities are deleted");
        }
        
        logger.LogInformation("[PASS] FullTree Entity Filter Is Fine");
    }
}