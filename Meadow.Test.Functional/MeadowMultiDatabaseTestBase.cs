using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Google.Protobuf.Reflection;
using Meadow.Requests.BuiltIn;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Transliteration;
using Meadow.Transliteration.Builtin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public abstract class MeadowMultiDatabaseTestBase : MeadowFunctionalTest
    {
        protected Dictionary<Type, List<object>> _seedData = new Dictionary<Type, List<object>>();

        private void RegisterSeededData(object data)
        {
            if (data is { } d)
            {
                var type = d.GetType();

                if (!_seedData.ContainsKey(type))
                {
                    _seedData.Add(type, new List<object>());
                }

                _seedData[type].Add(d);
            }
        }

        public List<T> GetSeededObjects<T>() => GetSeededObjects(typeof(T)).Select(i => (T)i).ToList();

        public List<object> GetSeededObjects(Type type) =>
            _seedData.ContainsKey(type) ? _seedData[type] : new List<object>();


        
        protected void Seed<T>(MeadowEngine engine, IEnumerable<T> seed) where T : class
        {
            var idLeaf = TypeIdentity.FindIdentityLeaf(typeof(T));

            Action<T, T> setId = (i, s) => { };
            if (idLeaf != null)
            {
                setId = (i, s) => idLeaf.Evaluator.Write(s, idLeaf.Evaluator.Read(i));
            }

            foreach (var item in seed)
            {
                var inserted = engine.PerformRequest(new InsertRequest<T>(item))
                    .FromStorage.FirstOrDefault();

                if (inserted == null)
                {
                    Console.WriteLine("PROBLEM SEEDING OBJECT");
                }
                else
                {
                    setId(inserted, item);

                    RegisterSeededData(item);
                }
            }
        }

        protected static void Index<T>(MeadowEngine engine, IEnumerable<T> seed)
        {
            var idLeaf = TypeIdentity.FindIdentityLeaf<T>();
            var idType = idLeaf.Type;
            var requestGeneric = typeof(IndexEntity<,>);
            var requestType = requestGeneric.MakeGenericType(typeof(T), idType);
            var constructor = requestType.GetConstructor(new Type[] { typeof(string), idType });
            var indexing = new IndexCorpusService<T>(new EnglishTransliterationsService());
            var genericSearchIndex = typeof(SearchIndex<>);
            var searchIndexType = genericSearchIndex.MakeGenericType(idType);
            var methodName = nameof(MeadowEngine.PerformRequest);
            var genericPerformMethod = typeof(MeadowEngine)
                .GetMethods()
                .FirstOrDefault(m => m.Name == methodName &&
                                     m.GetGenericArguments().Length == 2);
            var performMethod = genericPerformMethod!.MakeGenericMethod(searchIndexType, searchIndexType);
            foreach (var item in seed)
            {
                var corpus = indexing.GetIndexCorpus(item, false);
                var id = idLeaf.Evaluator.Read(item);
                var request = constructor!.Invoke(new object[] { corpus, id });

                performMethod!.Invoke(engine, new object[] { request, false });
            }
        }


        protected abstract void SelectDatabase();


        protected abstract void Main(MeadowEngine engine, ILogger logger);


        private bool AreEqual(object o1, object o2)
        {
            if (o1 == null && o2 == null)
            {
                return true;
            }

            if (o1 == null || o2 == null)
            {
                return false;
            }

            return o1.Equals(o2);
        }

        protected void CompareEntities<T>(T expected, T actual, bool ignoreId = true)
        {
            var idLeafName = TypeIdentity.FindIdentityLeaf(typeof(T))?.GetFullName();

            var ev = new ObjectEvaluator(typeof(T));

            IEnumerable<AccessNode> leaves = ignoreId
                ? ev.RootNode.GetDirectLeaves().Where(l => l.GetFullName() != idLeafName)
                : ev.RootNode.GetDirectLeaves();

            foreach (var leaf in leaves)
            {
                var ex = leaf.Evaluator.Read(expected);
                var ac = leaf.Evaluator.Read(actual);

                if (!AreEqual(ex, ac))
                {
                    throw new Exception($"Expected {leaf.GetFullName()} to be {ex}, " +
                                        $"but it was {ac}");
                }
            }
        }

        protected virtual LoggerAdapter OnLoggerConfiguration(LoggerAdapter logger)
        {
            return logger;
        }


        public override void Main()
        {
            SelectDatabase();

            var logger = new ConsoleLogger().Shorten().EnableAll();

            logger = OnLoggerConfiguration(logger);

            MeadowEngine.UseLogger(logger);

            var engine = CreateEngine();

            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }

            engine.CreateDatabase();

            engine.BuildUpDatabase();

            Main(engine, logger);
        }

        protected object ReadByHeadlessAddress<T>(string headlessAddress, object owner)
        {
            var ev = new ObjectEvaluator(typeof(T));

            var fullAddress = ev.RootNode.Name + "." + headlessAddress;

            var node = ev.Map.NodeByAddress(fullAddress);

            if (node != null)
            {
                return node.Evaluator.Read(owner);
            }

            return null;
        }
    }
}