using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional.TestEnvironment;

public class PersonsEnvironment : PersonUseCaseTestBase
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
        private PersonsEnvironment parent;
        private Person[] persons;
        public MeadowEngine Engine { get; private set; }

        public Environment(MeadowEngine engine, Person[] persons, PersonsEnvironment parent)
        {
            this.Engine = engine;
            this.persons = persons;
            this.parent = parent;
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

            return response as FindPagedRequest<TModel>;
        }

        public Person[] GetPersons(Func<Person, bool> predicate) => persons.Where(predicate).ToArray();
        public List<Person> GetSorted(Comparison<Person> compare) => Sort(persons, compare);
        public void Index<TModel>(IEnumerable<TModel> items) => MeadowMultiDatabaseTestBase.Index(Engine, items);

        public List<TModel> Update<TModel>(Func<TModel, bool> predicate, Action<TModel> update) where TModel : class, new()
        {
            var items = parent.GetSeededObjects<TModel>();

            var itemsToUpdate = items.Where(predicate).ToList();

            var updatedObjects = new List<TModel>();

            foreach (var model in itemsToUpdate)
            {
                update(model);

                var updated = Engine.PerformRequest(new UpdateRequest<TModel>(model)).FromStorage.FirstOrDefault();

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

        Seed(engine);

        env(new Environment(engine, Persons, this));
    }
}