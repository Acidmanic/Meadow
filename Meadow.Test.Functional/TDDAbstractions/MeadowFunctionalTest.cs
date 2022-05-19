using System;
using System.Collections;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Log;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.TDDAbstractions
{
    public abstract class MeadowFunctionalTest : IFunctionalTest
    {
        private readonly string _dbName;

        protected MeadowFunctionalTest(string dbName)
        {
            _dbName = dbName;
        }

        protected MeadowFunctionalTest()
        {
            _dbName = GetType().Name + "Db2BeDeleted";

            Console.WriteLine($@"****Database: '{_dbName}' is being used for this test.*****");
        }

        protected string GetConnectionString()
        {
            return "Server=localhost;" +
                   "User Id=sa; " +
                   "Password=never54aga.1n;" +
                   $@"Database={_dbName}; " +
                   "MultipleActiveResultSets=true";
        }

        protected MeadowEngine CreateEngine()
        {
            return new MeadowEngine(
                new MeadowConfiguration
                {
                    ConnectionString = GetConnectionString(),
                    BuildupScriptDirectory = "Scripts"
                },
                new ConsoleLogger());
        }

        protected void PrintObject(object o)
        {
            PrintObject("", o);
        }

        private void PrintObject(string indent, object obj)
        {
            if (obj is IEnumerable objects)
            {
                Console.WriteLine(indent + "[");
                foreach (var o in objects)
                {
                    PrintNonEnumerableObject(indent, o);
                }

                Console.WriteLine(indent + "]");
            }
            else
            {
                PrintNonEnumerableObject(indent, obj);
            }
        }

        private void PrintNonEnumerableObject(string indent, object o)
        {
            Line(indent, 30);

            var type = o.GetType();

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.CanRead)
                {
                    var name = property.Name;
                    var value = property.GetValue(o);

                    if (value == null || IsSingleLinable(property.PropertyType))
                    {
                        Console.WriteLine(indent + name + ": " + value);
                    }
                    else
                    {
                        Console.WriteLine(indent + property.Name);

                        PrintObject(indent + "    ", value);
                    }
                }
            }
        }

        private void Line(string caption, int lineLength)
        {
            var length = lineLength - caption.Length;

            string line = caption;

            for (int i = 0; i < length; i++)
            {
                line += "-";
            }

            Console.WriteLine(line);
        }

        private bool IsSingleLinable(Type type)
        {
            if (type.IsPrimitive || type.IsValueType)
            {
                return true;
            }

            if (type == typeof(string) || type == typeof(char))
            {
                return true;
            }

            return false;
        }

        private class Koon
        {
            public List<Person> People { get; set; }
        }

        protected void PrintTest()
        {
            var person = new Person
            {
                Age = 10,
                Id = 10,
                Job = new Job
                {
                    Id = 1010,
                    Title = "TenTinaTun",
                    JobDescription = "Stupid",
                    IncomeInRials = 10101010
                },
                Name = "Tutun",
                Surname = "Tatanian",
                JobId = 1010
            };

            var printTest = new Koon
            {
                People = new List<Person>
                {
                    person, person
                }
            };

            PrintObject(printTest);
        }

        public abstract void Main();
    }
}