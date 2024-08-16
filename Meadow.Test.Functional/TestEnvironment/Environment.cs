using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.TestEnvironment.Utility;
using Meadow.Transliteration;
using Meadow.Transliteration.Builtin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional.TestEnvironment;

public class Environment<TCaseProvider> : PersonUseCaseTestBase where TCaseProvider : ICaseDataProvider, new()
{
    protected override void SelectDatabase()
    {
        throw new NotImplementedException();
    }

    private void SelectDatabase(Databases database)
    {
        if (database == Databases.SqLite)
        {
            UseSqLite();
        }
        else if (database == Databases.MySql)
        {
            UseMySql();
        }
        else if (database == Databases.SqlServer)
        {
            UseSqlServer();
        }
        else if (database == Databases.Postgre)
        {
            UsePostgre();
        }
    }

    public ITransliterationService TransliterationService { get; set; } = new EnglishTransliterationsService();
    
    private Action<MeadowConfiguration> _updateConfigurations = c => { };


    public void RegulateMeadowConfigurations(Action<MeadowConfiguration> configure)
    {
        _updateConfigurations = configure;
    }

    private class Context : ISuitContext
    {
        private readonly ITransliterationService _transliterationService;

        public MeadowEngine Engine { get; }
        public CaseData Data { get; }

        public Context(MeadowEngine engine, CaseData data, ITransliterationService transliterationService)
        {
            this.Engine = engine;
            Data = data;
            _transliterationService = transliterationService;
        }

        public string[] Transliterate(params string[] searchTerms)
        {
            return searchTerms.Select(s => _transliterationService.Transliterate(s)).ToArray();
        }

        public FindPagedRequest<TModel> FindPaged<TModel>(Action<FilterQueryBuilder<TModel>> filter = null,
            int offset = 0, int size = 1000, Action<OrderSetBuilder<TModel>> order = null, params string[] searchTerms)
            where TModel : class
        {
            var filterQueryBuilder = new FilterQueryBuilder<TModel>();

            if (filter != null) filter(filterQueryBuilder);

            var ordersBuilder = new OrderSetBuilder<TModel>();

            if (order != null) order(ordersBuilder);

            var request = new FindPagedRequest<TModel>(filterQueryBuilder.Build(), offset, size, searchTerms,
                ordersBuilder.Build());

            var response = Engine
                .PerformRequest(request);

            if (response.Failed) throw response.FailureException;

            return response as FindPagedRequest<TModel>;
        }

        public void Index<TModel>(IEnumerable<TModel> items) =>
            IndexingUtilities.Index(Engine, items, _transliterationService);

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
        SelectDatabase(database);

        MeadowEngine.UseLogger(logger);

        var engine = CreateEngine(_updateConfigurations);

        if (engine.DatabaseExists())
        {
            engine.DropDatabase();
        }

        engine.CreateDatabase();

        engine.BuildUpDatabase();

        var dataProvider = new TCaseProvider();

        dataProvider.Initialize();

        var rawDataSets = dataProvider.SeedSet;

        SeedDataSets(engine, rawDataSets);

        var data = CaseData.Create(rawDataSets);

        env(new Context(engine, data,TransliterationService));
    }
}