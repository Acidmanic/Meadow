using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;
using Org.BouncyCastle.Crypto.Prng;

namespace Meadow.Test.Functional.TestEnvironment;

public class PersonsEnvironment<TCaseProvider> : PersonUseCaseTestBase where TCaseProvider:ICaseDataProvider , new()
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

    private Action<MeadowConfiguration> updateConfigurations = c => { };

    
    public void RegulateMeadowConfigurations(Action<MeadowConfiguration> configure)
    {
        updateConfigurations = configure;
    }

    private class Environment : IPersonsEnvironment
    {
        private PersonsEnvironment<TCaseProvider> parent;
        public MeadowEngine Engine { get; private set; }
        public CaseData Data { get; }

        public Environment(MeadowEngine engine, PersonsEnvironment<TCaseProvider> parent, CaseData data)
        {
            this.Engine = engine;
            this.parent = parent;
            Data = data;
        }

        public string[] Transliterate(params string[] searchTerms) => parent.Transliterate(searchTerms);

        public FindPagedRequest<TModel> FindPaged<TModel>(Action<FilterQueryBuilder<TModel>> filter = null, int offset = 0, int size = 1000, Action<OrderSetBuilder<TModel>> order = null, params string[] searchTerms) where TModel : class
        {
            var filterQueryBuilder = new FilterQueryBuilder<TModel>();

            if (filter != null) filter(filterQueryBuilder);

            var ordersBuilder = new OrderSetBuilder<TModel>();

            if (order != null) order(ordersBuilder);

            var request = new FindPagedRequest<TModel>(filterQueryBuilder.Build(), offset, size, searchTerms, ordersBuilder.Build());

            var response = Engine
                .PerformRequest(request);

            if (response.Failed) throw response.FailureException;
            
            return response as FindPagedRequest<TModel>;
        }

        public void Index<TModel>(IEnumerable<TModel> items) => MeadowMultiDatabaseTestBase.Index(Engine, items);

        public List<TModel> Update<TModel>(Func<TModel, bool> predicate, Action<TModel> update) where TModel : class, new()
        {
            var items = parent.GetSeededObjects<TModel>();

            var itemsToUpdate = items.Where(predicate).ToList();

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

       
        private List<TModel> Sort<TModel>(IEnumerable<TModel> items, Comparison<TModel> compare)
        {
            var list = new List<TModel>(items);

            list.Sort(compare);

            return list;
        }
    }


    public void Perform(Databases database, Action<IPersonsEnvironment> env)
        => Perform(database, new ConsoleLogger().Shorten().EnableAll(), env);

    public void Perform(Databases database, ILogger logger, Action<IPersonsEnvironment> env)
    {
        SelectDatabase(database);

        MeadowEngine.UseLogger(logger);

        var engine = CreateEngine(updateConfigurations);

        if (engine.DatabaseExists())
        {
            engine.DropDatabase();
        }

        engine.CreateDatabase();

        engine.BuildUpDatabase();

        var dataProvider = new TCaseProvider();

        dataProvider.Initialize();
        
        var rawDataSets = dataProvider.SeedSet;
        
        SeedDataSets(engine,rawDataSets);
        
        var data = CaseData.Create(rawDataSets);
        
        env(new Environment(engine, this,data));
    }
}