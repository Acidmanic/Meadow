using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd40FullTreeAccessNewMethod : MeadowFunctionalTest
    {
        private class ReadAllRequest<T> : MeadowRequest<MeadowVoid, T> where T : class, new()
        {
            public ReadAllRequest() : base(true)
            {
            }

            public override string RequestText
            {
                get
                {
                    var nc = Configuration.GetNameConvention<T>();

                    if (FullTreeReadWrite())
                    {
                        return nc.SelectAllProcedureNameFullTree;
                    }

                    return nc.SelectAllProcedureName;
                }
            }
        }

        private sealed class InsertRequest<T> : MeadowRequest<T, T> where T : class, new()
        {
            public InsertRequest(T model) : base(true)
            {
                ToStorage = model;
            }

            protected override void OnFieldManipulation(IFieldInclusionMarker<T> toStorage, IFieldInclusionMarker<T> fromStorage)
            {
                base.OnFieldManipulation(toStorage, fromStorage);

                var idLeaf = TypeIdentity.FindIdentityLeaf<T>();
                
                toStorage.Exclude(idLeaf.GetFullName());
            }

            public override string RequestText
            {
                get
                {
                    var nc = Configuration.GetNameConvention<T>();

                    return nc.InsertProcedureName;
                }
            }
        }

        private static void InsertAll<T>(MeadowEngine engine, IEnumerable<T> seed) where T : class, new()
        {
            foreach (var item in seed)
            {
                engine.PerformRequest(new InsertRequest<T>(item));
            }
        }

        private static Job J(string personName, long income)
        {
            return new Job
            {
                Title = personName + "'s Job",
                JobDescription = personName + "'s job description",
                IncomeInRials = income
            };
        }

        private static Person P(string name, string surname, int age, long jobId)
        {
            return new Person
                { Age = age, Name = name, Surname = surname, JobId = jobId };
        }

        private static Address A(int addressNumber, long personId)
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

        private static readonly Job[] Jobs = { J("Mani", 100), J("Mona", 200), J("Mina", 300), J("Farshid", 400), J("Farimehr", 500) };

        private static readonly Person[] Persons =
        {
            P("Mani", "Moayedi", 37, 1),
            P("Mona", "Moayedi", 42, 2),
            P("Mina", "Haddadi", 56, 3),
            P("Farshid", "Moayedi", 63, 4),
            P("Farimehr", "Ayerian", 21, 5),
        };

        private static readonly Address[] Addresses =
        {
            A(1, 1),
            A(1, 2), A(2, 2),
            A(1, 3), A(2, 3), A(3, 3),
            A(1, 4), A(2, 4), A(3, 4), A(4, 4),
            A(1, 5), A(2, 5), A(3, 5), A(4, 5), A(5, 5),
        };

        private static void Seed(MeadowEngine engine)
        {
            InsertAll(engine,Jobs);
            InsertAll(engine,Persons);
            InsertAll(engine,Addresses);
        }
        
        public override void Main()
        {
            UseSqlServer();

            var engine = CreateEngine();

            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }

            engine.CreateDatabase();

            engine.BuildUpDatabase();

            Seed(engine);

            var allPersons = engine.PerformRequest(new ReadAllRequest<Person>()).FromStorage;

            var allFullTreePersons = engine.PerformRequest(new ReadAllRequest<Person>(), true).FromStorage;
            
            
        }
    }
}