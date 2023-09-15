using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Search.Services;
using Meadow.Test.Functional.TDDAbstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;
using Org.BouncyCastle.Security;

namespace Meadow.Test.Functional
{
    public abstract class MeadowMultiDatabaseTestBase : MeadowFunctionalTest
    {
        protected static void InsertAll<T>(MeadowEngine engine, IEnumerable<T> seed) where T : class, new()
        {
            var idLeaf = TypeIdentity.FindIdentityLeaf(typeof(T));
            
            Action<T,T> setId = (i,s) => { };
            if (idLeaf != null)
            {
                setId = (i,s) => idLeaf.Evaluator.Write(s, idLeaf.Evaluator.Read(i));
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
                    setId(inserted,item);
                }
            }
        }

        protected static Job J(string personName, long income)
        {
            return new Job
            {
                Title = personName + "'s Job",
                JobDescription = personName + "'s job description",
                IncomeInRials = income
            };
        }

        protected static Person P(string name, string surname, int age, long jobId)
        {
            return new Person
                { Age = age, Name = name, Surname = surname, JobId = jobId };
        }

        protected static Address A(int addressNumber, long personId)
        {
            string[] counts = { "First", "Second", "Third", "Fourth", "Fifth" };
            addressNumber -= 1;
            return new Address
            {
                Block = addressNumber,
                City = counts[addressNumber] + " City",
                Plate = addressNumber,
                Street = counts[addressNumber] + " Street",
                AddressName = counts[addressNumber] + " Address For " + personId,
                PersonId = personId
            };
        }

        protected static readonly Job[] Jobs =
            { J("Mani", 100), J("Mona", 200), J("Mina", 300), J("Farshid", 400), J("Farimehr", 500) };

        protected static readonly Person[] Persons =
        {
            P("Mani", "Moayedi", 37, 1),
            P("Mona", "Moayedi", 42, 2),
            P("Mina", "Haddadi", 56, 3),
            P("Farshid", "Moayedi", 63, 4),
            P("Farimehr", "Ayerian", 21, 5),
        };

        protected static readonly Address[] Addresses =
        {
            A(1, 1),
            A(1, 2), A(2, 2),
            A(1, 3), A(2, 3), A(3, 3),
            A(1, 4), A(2, 4), A(3, 4), A(4, 4),
            A(1, 5), A(2, 5), A(3, 5), A(4, 5), A(5, 5),
        };

        protected static void Seed(MeadowEngine engine)
        {
            InsertAll(engine, Jobs);
            InsertAll(engine, Persons);
            InsertAll(engine, Addresses);
        }


        protected static void Index<T>(MeadowEngine engine,IEnumerable<T> seed)
        {
            var idLeaf = TypeIdentity.FindIdentityLeaf<T>();
            var idType = idLeaf.Type;
            var requestGeneric = typeof(IndexEntity<,>);
            var requestType = requestGeneric.MakeGenericType(typeof(T), idType);
            var constructor = requestType.GetConstructor(new Type[]{typeof(string),idType});
            var indexing = new IndexingService<T>(new EnglishTransliterationsService());
            var genericSearchIndex = typeof(SearchIndex<>);
            var searchIndexType = genericSearchIndex.MakeGenericType(idType);
            var methodName = nameof(MeadowEngine.PerformRequest); 
            var genericPerformMethod = typeof(MeadowEngine)
                    .GetMethods()
                    .FirstOrDefault(m => m.Name==methodName && 
                                         m.GetGenericArguments().Length==2);
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


        protected void Log(ILogger logger, Person p)
        {
            var ft = "";
            if (p.Job != null)
            {
                ft += $"Job:{p.Job.Title}+{p.Job.IncomeInRials} ";
            }

            if (p.Addresses.Count > 0)
            {
                ft += $"LAdd: {p.Addresses.Last().AddressName}";
            }

            logger.LogInformation($"{p.Name} {p.Surname} : {p.Age} years old. JI-{p.JobId} {ft}");
        }

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

        public override void Main()
        {
            SelectDatabase();

            var logger = new ConsoleLogger().Shorten().EnableAll();

            MeadowEngine.UseLogger(logger);

            var engine = CreateEngine();

            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }

            engine.CreateDatabase();

            engine.BuildUpDatabase();

            Seed(engine);

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