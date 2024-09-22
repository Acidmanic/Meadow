using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.Requests;
using Meadow.Requests.BuiltIn;
using Meadow.Requests.BuiltIn.Dtos;
using Meadow.Requests.GenericEventStreamRequests;
using Meadow.Requests.GenericEventStreamRequests.Models;
using Meadow.Scaffolding.Attributes;
using Meadow.Test.Functional.TestEnvironment.Extensions;
using Meadow.Test.Functional.TestEnvironment.Utility;
using Meadow.Test.Shared;
using Meadow.Transliteration;
using Meadow.Transliteration.Builtin;
using Meadow.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional.TestEnvironment;

public class Environment<TCaseProvider> where TCaseProvider : ICaseDataProvider, new()
{
    private readonly string _scriptsDirectory;
    private readonly string? _suggestedDatabaseName;
    public ITransliterationService TransliterationService { get; set; } = new EnglishTransliterationsService();

    private Action<MeadowConfiguration> _updateConfigurations = _ => { };

    private readonly Dictionary<string, string> _scriptOverrides = new();

    public void RegulateMeadowConfigurations(Action<MeadowConfiguration> configure)
    {
        _updateConfigurations = configure;
    }

    public void OverrideScriptFile(string fileName, string content)
    {
        _scriptOverrides[fileName] = content;
    }

    public Environment() : this("MacroScripts")
    {
    }

    public Environment(string scriptsDirectory, string? suggestedDatabaseName = null)
    {
        _scriptsDirectory = scriptsDirectory;
        _suggestedDatabaseName = suggestedDatabaseName;
    }

    private class Context : ISuitContext
    {
        public MeadowEngine Engine { get; }
        public CaseData Data { get; }

        public string DatabaseName { get; }

        public MeadowConfiguration MeadowConfiguration { get; }

        public ILogger Logger { get; }

        public Context(MeadowEngine engine, CaseData data, string databaseName, MeadowConfiguration meadowConfiguration, ILogger logger)
        {
            Engine = engine;
            Data = data;
            DatabaseName = databaseName;
            MeadowConfiguration = meadowConfiguration;
            Logger = logger;
        }


        public FindPagedRequest<TModel> FindPaged<TModel>(Action<FilterQueryBuilder<TModel>>? filter = null,
            int offset = 0, int size = 1000, Action<OrderSetBuilder<TModel>>? order = null,
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

            var response = Engine.PerformRequest(request, fullTree);

            if (response.Failed) throw response.FailureException;

            return (response as FindPagedRequest<TModel>)!;
        }

        public void Index<TModel>(IEnumerable<TModel> items) => IndexingUtilities.Index(Engine, items);

        public List<TModel> Update<TModel>(Func<TModel, bool> predicate, Action<TModel> update)
            where TModel : class
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

        public List<TModel> Save<TModel>(Func<TModel, bool> predicate, Action<TModel> update,
            string? collectionName = null) where TModel : class
        {
            var itemsToSaved = Data.Get(predicate).ToList();

            var savedObjects = new List<TModel>();

            foreach (var model in itemsToSaved)
            {
                update(model);

                var response = Engine.PerformRequest(new SaveRequest<TModel>(model, collectionName));

                if (response.Failed) throw response.FailureException;

                var saved = response.FromStorage.FirstOrDefault();

                if (saved is { } s)
                {
                    savedObjects.Add(s);
                }
            }

            return savedObjects;
        }

        public TModel? Save<TModel>(TModel model, string? collectionName = null) where TModel : class
        {
            var response = Engine.PerformRequest(new SaveRequest<TModel>(model, collectionName));

            if (response.Failed) throw response.FailureException;

            return response.FromStorage.FirstOrDefault();
        }

        public ReadByIdRequest<TModel, TId> ReadById<TModel, TId>(TId id, bool fullTree = false)
            where TModel : class
            => (ReadByIdRequest<TModel, TId>)PerformRequest(new ReadByIdRequest<TModel, TId>(id), fullTree);

        public ReadAllRequest<TModel> ReadAll<TModel>(bool fullTree = false) where TModel : class
            => (ReadAllRequest<TModel>)PerformRequest(new ReadAllRequest<TModel>(), fullTree);

        public DeleteById<TEntity, TId> DeleteById<TEntity, TId>(TId id)
            => (DeleteById<TEntity, TId>)PerformRequest(new DeleteById<TEntity, TId>(id));

        public List<StreamEvent> EventStreamRead<TEvent, TEventId, TStreamId>()
        {
            return PerformRequest(new ReadAllStreamsRequest<TEvent, TEventId, TStreamId>())
                .FromStorage.ToStreamEvents(MeadowConfiguration);
        }

        public List<StreamEvent> EventStreamRead<TEvent, TEventId, TStreamId>(TStreamId streamId)
        {
            return PerformRequest(new ReadStreamByStreamIdRequest<TEvent, TEventId, TStreamId>(streamId))
                .FromStorage.ToStreamEvents(MeadowConfiguration);
        }

        public List<StreamEvent> EventStreamRead<TEvent, TEventId, TStreamId>(TEventId baseEventId, long count)
        {
            return PerformRequest(new ReadAllStreamsChunksRequest<TEvent, TEventId, TStreamId>(baseEventId, count))
                .FromStorage.ToStreamEvents(MeadowConfiguration);
        }

        public List<StreamEvent> EventStreamRead<TEvent, TEventId, TStreamId>(TStreamId streamId, TEventId baseEventId, long count)
        {
            return PerformRequest(new ReadStreamChunkByStreamIdRequest<TEvent, TEventId, TStreamId>(streamId, baseEventId, count))
                .FromStorage.ToStreamEvents(MeadowConfiguration);
        }

        public FieldRangeDto<TField>? Range<TEntity, TField>(Expression<Func<TEntity, TField>> selector) => PerformRequest(new RangeRequest<TEntity, TField>(selector)).FromStorage.FirstOrDefault();

        public List<TField> Existings<TEntity, TField>(Expression<Func<TEntity, TField>> selector)
            => PerformRequest(new ExistingValuesRequest<TEntity, TField>(selector)).FromStorage
                .Select(vd => vd.Value).ToList();

        public List<TReturn> DirectPerform<TReturn>(string sql)
            where TReturn : class
            => PerformRequest(new DiscouragedDirectSqlRequest<TReturn>(sql))
                .FromStorage;

        public void DirectPerform(string sql) => PerformRequest(new DiscouragedDirectSqlRequest(sql));


        private MeadowRequest<TIn, TOut> PerformRequest<TIn, TOut>
            (MeadowRequest<TIn, TOut> request, bool fullTree = false) where TOut : class
        {
            var response = Engine.PerformRequest(request, fullTree);

            if (response.Failed)
            {
                throw response.FailureException;
            }

            return response;
        }
    }


    public void Perform(Databases database, Action<ISuitContext> env)
        => Perform(database, new ConsoleLogger().Shorten().EnableAll(), env);


    public void Perform(Databases database, ILogger logger, Action<ISuitContext> env)
    {
        var engineSetup = new MeadowEngineSetup(_suggestedDatabaseName);

        engineSetup.SelectDatabase(database, _scriptsDirectory);

        MeadowEngine.UseLogger(logger);

        var engine = engineSetup.CreateEngine(c =>
        {
            c.SetTransliterationService(TransliterationService);

            OverrideScripts(c);

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

        // var rawDataSets = dataProvider.SeedSet;

        // var data = CaseData.Create(rawDataSets);

        var data = Seed(dataProvider, engine, engineSetup.Configuration);
        
        var context = new Context(engine, data, engineSetup.DatabaseName, engineSetup.Configuration, logger);

        //SeedingUtilities.SeedCaseData(engine, data);

        dataProvider.PostSeeding();

        env(context);
    }


    private CaseData Seed(TCaseProvider provider, MeadowEngine engine, MeadowConfiguration configuration)
    {
        provider.Initialize();

        var data = CaseData.Create(provider.SeedSet);

        var seedsByType = SeedObjectsByType(data.SeedsByType,engine,configuration);
        var eventsByStreamId = SeedEventsByStreamId(data.EventsByStreamId,engine,configuration);

        return new CaseData(seedsByType, eventsByStreamId);
    }
    
    
    private Dictionary<Type, List<object>> SeedObjectsByType(IReadOnlyDictionary<Type, List<object>> objectsByType, MeadowEngine engine, MeadowConfiguration configuration)
    {
      
        var seedsByType = new Dictionary<Type, List<object>>();

        foreach (var typeValue in objectsByType)
        {
            if(!seedsByType.ContainsKey(typeValue.Key)) seedsByType.Add(typeValue.Key, new List<object>());
            
            foreach (object o in typeValue.Value)
            {
                var insertScript = configuration.TranslateInsert(typeValue.Key, o);

                var request = new DiscouragedDirectSqlRequest(insertScript);

                engine.Perform(request, false);
            }
        }
        
        foreach (var typeValue in objectsByType)
        {
            var readAllSql = configuration.TranslateSelectAll(typeValue.Key, true);

            var readAllRequest = CreateRequest(readAllSql, typeValue.Key);

            if (readAllRequest is { } raRequest)
            {
                var readAllResponse = engine.Perform(raRequest, true);

                if (readAllResponse.Success)
                {
                    seedsByType[typeValue.Key].AddRange(readAllResponse.FromStorage);
                }
            }
        }

        return seedsByType;
    }
    
    private Dictionary<object, List<StreamEvent>> SeedEventsByStreamId(IReadOnlyDictionary<object, List<StreamEvent>> dataEventsByStreamId, MeadowEngine engine, MeadowConfiguration configuration)
    {
        var eventsByStreamId = new Dictionary<object, List<StreamEvent>>();
        
        foreach (var sIdEvent in dataEventsByStreamId)
        {
            var streamId = sIdEvent.Key;
            
            if(!eventsByStreamId.ContainsKey(streamId)) eventsByStreamId.Add(streamId, new List<StreamEvent>());

            foreach (var streamEvent in sIdEvent.Value)
            {
                var entry = new ObjectEntry<object, object>();
                var prefInfo = EventStreamPreferencesInfo.FromType(streamEvent.EventConcreteType);
                var serInfo = EventStreamSerializationInfo.FromType(streamEvent.EventConcreteType);
                
                entry.StreamId = streamId;
                entry.AssemblyName = streamEvent.EventConcreteType.AssemblyQualifiedName!;
                entry.EventId = streamEvent.EventId;
                entry.TypeName = streamEvent.EventConcreteType.FullName!;
                entry.SerializedValue = MeadowConfiguration.Null.EventSerialization.Serialize(
                    streamEvent.Event, serInfo.Encoding, serInfo.Compression, serInfo.CompressionLevel).Result;

                var tableName = configuration.GetNameConvention(prefInfo.Value.EventAbstraction).EventStreamTableName;
                
                var insertSql = configuration.TranslateInsert(entry.GetType(), entry, tableName);
                
                var request = new DiscouragedDirectSqlRequest(insertSql);

                engine.Perform(request, false);

            }
        }
        
        return eventsByStreamId;
    }


    private object? CreateRequest(string sql, Type returnType, bool fullTree = false)
    {
        var requestGenericType = typeof(DiscouragedDirectSqlRequest<>);

        var requestType = requestGenericType.MakeGenericType(returnType);

        var constructor = requestType.GetConstructor(new Type[] { typeof(string), typeof(object) });

        if (constructor is { } c)
        {
            try
            {
                var request = c.Invoke(new object[] { sql, fullTree });

                return request;
            }
            catch (Exception e)
            {
                /* Ignore */
            }
        }

        return null;
    }

    private void OverrideScripts(MeadowConfiguration configuration)
    {
        foreach (var scriptOverride in _scriptOverrides)
        {
            var file = Path.Combine(configuration.BuildupScriptDirectory, scriptOverride.Key);

            File.Delete(file);

            var content = scriptOverride.Value;

            File.WriteAllText(file, content);
        }
    }
}