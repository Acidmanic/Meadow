using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Examples.Common;
using Meadow.Configuration;
using Meadow.MySql;
using Meadow.Postgre;
using Meadow.SQLite;
using Meadow.SqlServer;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Utility;
using Xunit;

namespace Meadow.Test.Functional.TestEnvironment.Utility;

public class MeadowEngineSetup
{
    private string _connectionString;

    // private string _scriptsDirectory;
    private readonly List<Assembly> _meadowConfigurationAssemblies = new List<Assembly>();

    public string DatabaseName { get; private set; }
    public MeadowConfiguration Configuration { get; private set; }

    public MeadowEngineSetup()
    {
        DatabaseName = ProvideDataBaseName();
    }


    private Type GetTestSuitType()
    {
        var stack = new StackTrace();

        foreach (var stackFrame in stack.GetFrames())
        {
            if (stackFrame.GetMethod() is { } m)
            {
                if (m.GetCustomAttribute<FactAttribute>() is { }
                    || m.GetCustomAttribute<TheoryAttribute>() is { })
                {
                    if (m.DeclaringType is { } t) return t;
                }
            }
        }

        return GetType();
    }

    private string ProvideDataBaseName() => GetTestSuitType().Name + "Db2BeDeleted";


    private void UseSqLite(string scriptsDirectory = "MacroScripts")
    {
        _meadowConfigurationAssemblies.Clear();
        _meadowConfigurationAssemblies.Add(Assembly.GetEntryAssembly());
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMeadowAssembly());
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetSqLiteMeadowAssembly());

        var executablePath = new FileInfo(typeof(MeadowFunctionalTest).Assembly.Location).Directory?.FullName
                             ?? Environment.CurrentDirectory;
        
        _connectionString = ExampleConnectionString.GetSqLiteConnectionString(DatabaseName,executablePath);

        MeadowEngine.UseDataAccess(new CoreProvider<SqLiteDataAccessCore>());

        DatabaseName = "SqLite";

        UpdateConfigurations(scriptsDirectory);
    }

    private void UseMySql(string scriptsDirectory = "MacroScripts")
    {
        _meadowConfigurationAssemblies.Clear();
        _meadowConfigurationAssemblies.Add(Assembly.GetEntryAssembly());
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMeadowAssembly());
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMySqlMeadowAssembly());

        _connectionString = ExampleConnectionString.GetMySqlConnectionString(DatabaseName);

        MeadowEngine.UseDataAccess(new CoreProvider<MySqlDataAccessCore>());

        DatabaseName = "My Sql";

        UpdateConfigurations(scriptsDirectory);
    }

    private void UseSqlServer(string scriptsDirectory = "MacroScripts")
    {
        _meadowConfigurationAssemblies.Clear();
        _meadowConfigurationAssemblies.Add(Assembly.GetEntryAssembly());
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMeadowAssembly());
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetSqlServerMeadowAssembly());

        _connectionString = ExampleConnectionString.GetSqlServerConnectionString(DatabaseName);

        MeadowEngine.UseDataAccess(new CoreProvider<SqlServerDataAccessCore>());

        DatabaseName = "Sql Server";

        UpdateConfigurations(scriptsDirectory);
    }

    private void UsePostgre(string scriptsDirectory = "MacroScripts")
    {
        _meadowConfigurationAssemblies.Clear();
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMeadowAssembly());
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetPostgreMeadowAssembly());

        _connectionString = ExampleConnectionString.GetPostgresConnectionString(DatabaseName);

        MeadowEngine.UseDataAccess(new CoreProvider<PostgreDataAccessCore>());

        DatabaseName = "Postgre";

        UpdateConfigurations(scriptsDirectory);
    }

    private void UpdateConfigurations(string scriptsDirectory)
    {
        var executablePath = new FileInfo(typeof(MeadowFunctionalTest).Assembly.Location).Directory?.FullName
                             ?? Environment.CurrentDirectory;

        var sd = Path.Combine(executablePath, scriptsDirectory);

        _meadowConfigurationAssemblies.Add(typeof(MeadowFunctionalTest).Assembly);

        Configuration = new MeadowConfiguration
        {
            ConnectionString = _connectionString,
            BuildupScriptDirectory = sd,
            MacroPolicy = MacroPolicies.InterpretAtRuntimeWriteDebugFiles,
            MacroContainingAssemblies = new List<Assembly>(_meadowConfigurationAssemblies)
        };
    }

    public void SelectDatabase(Databases database, string scriptsDirectory = "MacroScripts")
    {
        if (database == Databases.SqLite)
        {
            UseSqLite(scriptsDirectory);
        }
        else if (database == Databases.MySql)
        {
            UseMySql(scriptsDirectory);
        }
        else if (database == Databases.SqlServer)
        {
            UseSqlServer(scriptsDirectory);
        }
        else if (database == Databases.Postgre)
        {
            UsePostgre(scriptsDirectory);
        }
    }


    public MeadowEngine CreateEngine(Action<MeadowConfiguration> configure = null)
    {
        if (configure is { } c) c(Configuration);

        return new MeadowEngine(Configuration);
    }
}