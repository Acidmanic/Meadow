using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Examples.Common;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.MySql;
using Meadow.Scaffolding.Macros;
using Meadow.SqlServer;
using Meadow.Utility;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional.TDDAbstractions
{
    public abstract class MeadowFunctionalTest : IFunctionalTest
    {
        public class TestLogger : ILogger
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                var message = logLevel.ToString();

                message += ": " + formatter(state, exception);

                Console.WriteLine(message);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }

        protected readonly string DbName;
        protected string ConnectionString;
        protected string ScriptsDirectory;
        protected readonly List<Assembly> MeadowConfigurationAssemblies = new List<Assembly>();

        protected MeadowFunctionalTest(string dbName)
        {
            DbName = dbName;
        }

        protected MeadowFunctionalTest()
        {
            new TestLogger().UseForMeadow();

            DbName = GetType().Name + "Db2BeDeleted";

            Console.WriteLine($@"****Database: '{DbName}' is being used for this test.*****");
        }

        protected void UseMySql()
        {
            
            MeadowConfigurationAssemblies.Clear();
            MeadowConfigurationAssemblies.Add(Assembly.GetEntryAssembly());
            MeadowConfigurationAssemblies.Add(typeof(IMacro).Assembly);
            MeadowConfigurationAssemblies.Add(typeof(MySqlDataAccessCore).Assembly);
            
            ScriptsDirectory = "MySqlScripts";
            
            ConnectionString= ExampleConnectionString.GetMySqlConnectionString(DbName);
            
            MeadowEngine.UseDataAccess(new CoreProvider<MySqlDataAccessCore>());
        }

        protected MeadowEngine CreateEngine()
        {
            return new MeadowEngine(
                new MeadowConfiguration
                {
                    ConnectionString = ConnectionString,
                    BuildupScriptDirectory = ScriptsDirectory,
                    MacroPolicy = MacroPolicies.UpdateScripts,
                    MacroContainingAssemblies = new List<Assembly>(MeadowConfigurationAssemblies)
                });
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

            engine.UseSqlServer();

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