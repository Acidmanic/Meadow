using System;
using System.Linq;
using Acidmanic.Utilities.Filtering.Utilities;
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


    private class Environment : IPersonsEnvironment
    {
        private Person[] persons;

        public Environment(MeadowEngine engine, Person[] persons)
        {
            Engine = engine;
            this.persons = persons;
        }

        public string[] Transliterate(object searchTerms)
        {
            throw new NotImplementedException();
        }

        public MeadowEngine Engine { get; }


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
    }


    public void Perform(Databases database, Action<IPersonsEnvironment> env)
        => Perform(database, new ConsoleLogger().Shorten().EnableAll(), env);

    public void Perform(Databases database, ILogger logger, Action<IPersonsEnvironment> env)
    {
        SelectDatabase(database);

        MeadowEngine.UseLogger(logger);

        var engine = CreateEngine();

        if (engine.DatabaseExists())
        {
            engine.DropDatabase();
        }

        engine.CreateDatabase();

        engine.BuildUpDatabase();

        Seed(engine);

        env(new Environment(engine,Persons));
    }
}