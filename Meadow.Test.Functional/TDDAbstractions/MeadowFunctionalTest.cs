using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Meadow.Configuration;
using Meadow.Log;
using Meadow.Reflection;
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
            if (obj == null)
            {
                Console.WriteLine("[NULL]");
                return;
            }

            if (TypeCheck.IsCollection(obj.GetType()) && obj is IEnumerable objects)
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

                    if (value == null || IsSingleLinable(value.GetType()))
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
        
        public abstract void Main();

        protected MeadowEngine SetupClearDatabase(bool fromScratch = true)
        {
            var engine = CreateEngine();

            if (fromScratch)
            {
                if (engine.DatabaseExists())
                {
                    engine.DropDatabase();
                }

                engine.CreateDatabase();    
            }
            engine.BuildUpDatabase();

            return engine;
        }
    }
}