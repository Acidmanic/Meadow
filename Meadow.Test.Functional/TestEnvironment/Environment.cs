using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Requests.BuiltIn;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.TestEnvironment.Utility;
using Meadow.Transliteration;
using Meadow.Transliteration.Builtin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional.TestEnvironment;

public class Environment<TCaseProvider> where TCaseProvider : ICaseDataProvider, new()
{
    public ITransliterationService TransliterationService { get; set; } = new EnglishTransliterationsService();

    private Action<MeadowConfiguration> _updateConfigurations = _ => { };


    public void RegulateMeadowConfigurations(Action<MeadowConfiguration> configure)
    {
        _updateConfigurations = configure;
    }

    private class Context : ISuitContext
    {

        public MeadowEngine Engine { get; }
        public CaseData Data { get; }

        public string DatabaseName { get; }
        
        public Context(MeadowEngine engine, CaseData data,  string databaseName)
        {
            Engine = engine;
            Data = data;
            DatabaseName = databaseName;
        }


        public FindPagedRequest<TModel> FindPaged<TModel>(Action<FilterQueryBuilder<TModel>> filter = null,
            int offset = 0, int size = 1000, Action<OrderSetBuilder<TModel>> order = null,
            bool fullTree = false,
            params string[] searchTerms)
            where TModel : class
        {
            var filterQueryBuilder = new FilterQueryBuilder<TModel>();

            if (filter != null) filter(filterQueryBuilder);

            var ordersBuilder = new OrderSetBuilder<TModel>();

            if (order != null) order(ordersBuilder);

            var request = new FindPagedRequest<TModel>(filterQueryBuilder.Build(), offset, size, searchTerms,
                ordersBuilder.Build());

            var response = Engine.PerformRequest(request,fullTree);

            if (response.Failed) throw response.FailureException;

            return response as FindPagedRequest<TModel>;
        }

        public void Index<TModel>(IEnumerable<TModel> items) =>
            IndexingUtilities.Index(Engine, items);

        public List<TModel> Update<TModel>(Func<TModel, bool> predicate, Action<TModel> update)
            where TModel : class, new()
        {
            var itemsToUpdate = Data.Get(predicate).ToList();

            var updatedObjects = new List<TModel>();

            foreach (var model in itemsToUpdate)
            {
                update(model);

                var response = Engine.PerformRequest(new UpdateRequest<TModel>(model));

                if (response.Failed) throw response.FailureException;

                var updated = response.FromStorage.FirstOrDefault();

                if (updated is { } u)
                {
                    updatedObjects.Add(u);
                }
            }

            return updatedObjects;
        }
    }


    public void Perform(Databases database, Action<ISuitContext> env)
        => Perform(database, new ConsoleLogger().Shorten().EnableAll(), env);
    

    public void Perform(Databases database, ILogger logger, Action<ISuitContext> env)
    {
        var engineSetup = new MeadowEngineSetup();

        engineSetup.SelectDatabase(database);

        MeadowEngine.UseLogger(logger);

        var engine = engineSetup.CreateEngine(c =>
        {
            
            _updateConfigurations(c);
        });
        
        if (engine.DatabaseExists())
        {
            logger.LogInformation("Dropping Existing Database...");
            
            engine.DropDatabase();
        }
        else
        {
            logger.LogInformation("No Database Has been found");
        }
        
        logger.LogInformation("Creating New Database Instance");

        engine.CreateDatabase();

        engine.BuildUpDatabase();

        var dataProvider = new TCaseProvider();

        dataProvider.Initialize();

        var rawDataSets = dataProvider.SeedSet;

        SeedingUtilities.SeedDataSets(engine, rawDataSets);
        
        dataProvider.PostSeeding();

        var data = CaseData.Create(rawDataSets);

        env(new Context(engine, data, engineSetup.DatabaseName));
    }
}