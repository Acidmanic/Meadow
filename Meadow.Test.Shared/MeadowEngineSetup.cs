using System.Diagnostics;
using System.Reflection;
using Examples.Common;
using Meadow.Configuration;
using Meadow.MySql;
using Meadow.Postgre;
using Meadow.SQLite;
using Meadow.SqlServer;
using Meadow.Utility;
using Xunit;

namespace Meadow.Test.Shared;

public class MeadowEngineSetup
{
    private string _connectionString;
    private readonly string? _suggestedDataBaseName;

    // private string _scriptsDirectory;
    private readonly List<Assembly> _meadowConfigurationAssemblies = new List<Assembly>();

    public string DatabaseName { get; }

    public string DatabaseDisplayName { get; private set; } = string.Empty;
    
    public MeadowConfiguration Configuration { get; private set; }

    public MeadowEngineSetup(string? suggestedDataBaseName=null)
    {
        _suggestedDataBaseName = suggestedDataBaseName;
        DatabaseName = ProvideDataBaseName();
    }


    private class MarkedType
    {
        public Type Type { get; set; }
        public bool Marked { get; set; }
    }

    
    private Type GetTestSuitType()
    {
        var stack = new StackTrace();

        var markedMethods =
            stack.GetFrames().Select(f => f.GetMethod())
                .Where(m => m != null)
                .Select(m => m!)
                .Where(m => m.DeclaringType!=null)
                .Select(m => new
                    MarkedType{
                    Type = m.DeclaringType!, Marked = (m.GetCustomAttribute<FactAttribute>() is { }
                                                      || m.GetCustomAttribute<TheoryAttribute>() is { })
                })
                .ToList();

        var hit = false;

        foreach (var markedMethod in markedMethods)
        {
            hit |= markedMethod.Marked;
            markedMethod.Marked = hit;
        }

        var found =
            markedMethods.FirstOrDefault(m => m.Marked && !m.Type.IsAbstract && !m.Type.IsGenericType);

        if (found is { } f) return f.Type;

        return GetType();
    }

    private string ProvideDataBaseName() => (_suggestedDataBaseName ?? GetTestSuitType().Name) + "Db2BeDeleted";


    private void UseSqLite(string scriptsDirectory = "MacroScripts")
    {
        _meadowConfigurationAssemblies.Clear();
        _meadowConfigurationAssemblies.Add(Assembly.GetEntryAssembly());
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMeadowAssembly());
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetSqLiteMeadowAssembly());

        var executablePath = new FileInfo(typeof(MeadowEngineSetup).Assembly.Location).Directory?.FullName
                             ?? Environment.CurrentDirectory;
        
        _connectionString = ExampleConnectionString.GetSqLiteConnectionString(DatabaseName,executablePath);

        MeadowEngine.UseDataAccess(new CoreProvider<SqLiteDataAccessCore>());

        DatabaseDisplayName = "SqLite";

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

        DatabaseDisplayName = "My Sql";

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

        DatabaseDisplayName = "Sql Server";

        UpdateConfigurations(scriptsDirectory);
    }

    private void UsePostgre(string scriptsDirectory = "MacroScripts")
    {
        _meadowConfigurationAssemblies.Clear();
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetMeadowAssembly());
        _meadowConfigurationAssemblies.Add(TheMeadow.Anchor.GetPostgreMeadowAssembly());

        _connectionString = ExampleConnectionString.GetPostgresConnectionString(DatabaseName);

        MeadowEngine.UseDataAccess(new CoreProvider<PostgreDataAccessCore>());

        DatabaseDisplayName = "Postgre";

        UpdateConfigurations(scriptsDirectory);
    }

    private void UpdateConfigurations(string scriptsDirectory)
    {
        var executablePath = new FileInfo(typeof(MeadowEngineSetup).Assembly.Location).Directory?.FullName
                             ?? Environment.CurrentDirectory;

        var sd = Path.Combine(executablePath, scriptsDirectory);

        _meadowConfigurationAssemblies.Add(typeof(MeadowEngineSetup).Assembly);

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