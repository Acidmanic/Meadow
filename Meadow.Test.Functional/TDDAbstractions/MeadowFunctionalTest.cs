using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Acidmanic.Utilities.Reflection;
using Examples.Common;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.MySql;
using Meadow.Postgre;
using Meadow.Scaffolding.Macros;
using Meadow.SQLite;
using Meadow.SqlServer;
using Meadow.Test.Functional.Models;
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
        protected string DatabaseName { get; private set; }
        
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
        

        protected void UseMySql(string scriptsDirectory="MacroScripts")
        {
            MeadowConfigurationAssemblies.Clear();
            MeadowConfigurationAssemblies.Add(Assembly.GetEntryAssembly());
            MeadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMeadowAssembly());
            MeadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMySqlMeadowAssembly());

            ScriptsDirectory = scriptsDirectory;

            ConnectionString = ExampleConnectionString.GetMySqlConnectionString(DbName);

            MeadowEngine.UseDataAccess(new CoreProvider<MySqlDataAccessCore>());

            DatabaseName = "My Sql";
            
            UpdateConfigurations();
        }

        protected void UseSqlServer(string scriptsDirectory="MacroScripts")
        {
            MeadowConfigurationAssemblies.Clear();
            MeadowConfigurationAssemblies.Add(Assembly.GetEntryAssembly());
            MeadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMeadowAssembly());
            MeadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetSqlServerMeadowAssembly());

            ScriptsDirectory = scriptsDirectory;

            ConnectionString = ExampleConnectionString.GetSqlServerConnectionString(DbName);

            MeadowEngine.UseDataAccess(new CoreProvider<SqlServerDataAccessCore>());
            
            DatabaseName = "Sql Server";
            
            UpdateConfigurations();
        }

        protected void UsePostgre(string scriptsDirectory="MacroScripts")
        {
            MeadowConfigurationAssemblies.Clear();
            MeadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMeadowAssembly());
            MeadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetPostgreMeadowAssembly());

            ScriptsDirectory = scriptsDirectory;

            ConnectionString = ExampleConnectionString.GetPostgresConnectionString(DbName);

            MeadowEngine.UseDataAccess(new CoreProvider<PostgreDataAccessCore>());
            
            DatabaseName = "Postgre";
            
            UpdateConfigurations();
        }

        protected void UseSqLite(string scriptsDirectory="MacroScripts")
        {
            MeadowConfigurationAssemblies.Clear();
            MeadowConfigurationAssemblies.Add(Assembly.GetEntryAssembly());
            MeadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMeadowAssembly());
            MeadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetSqLiteMeadowAssembly());

            ConnectionString = ExampleConnectionString.GetSqLiteConnectionString(DbName);

            MeadowEngine.UseDataAccess(new CoreProvider<SqLiteDataAccessCore>());
            
            DatabaseName = "SqLite";

            UpdateConfigurations();
        }

        private void UpdateConfigurations()
        {
            
            var executablePath = new FileInfo(typeof(MeadowFunctionalTest).Assembly.Location).Directory?.FullName
                                 ?? Environment.CurrentDirectory;
            
            var sd = ScriptsDirectory = Path.Combine(executablePath, ScriptsDirectory);
            
            MeadowConfigurationAssemblies.Add(typeof(MeadowFunctionalTest).Assembly);
            
            Configuration = new MeadowConfiguration
            {
                ConnectionString = ConnectionString,
                BuildupScriptDirectory = sd,
                MacroPolicy = MacroPolicies.UpdateScripts,
                MacroContainingAssemblies = new List<Assembly>(MeadowConfigurationAssemblies)
            };
            
        }

        protected MeadowConfiguration Configuration { get; private set; }

        
        protected virtual MeadowConfiguration RegulateConfigurations(MeadowConfiguration configurations)
        {
            return configurations;
        }
        
        protected MeadowEngine CreateEngine(Action<MeadowConfiguration> configure = null)
        {
            configure ??= c => RegulateConfigurations(c);

            configure(Configuration);
            
            return new MeadowEngine(Configuration);
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