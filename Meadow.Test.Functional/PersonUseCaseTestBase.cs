using System.Linq;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public abstract class PersonUseCaseTestBase : MeadowMultiDatabaseTestBase
    {

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


        protected override void Main(MeadowEngine engine, ILogger logger)
        {
            Seed(engine);
        }


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
    }
}