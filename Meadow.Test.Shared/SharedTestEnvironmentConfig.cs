using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;

namespace Meadow.Test.Shared;

public class SharedTestEnvironmentConfig
{
    private sealed record EnvironmentData(string DatabaseType);

    private static SharedTestEnvironmentConfig? _environmentDatabase = null;
    private static readonly object _lock = new();

    public static SharedTestEnvironmentConfig Instance
    {
        get
        {
            lock (_lock)
            {
                if (_environmentDatabase is null)
                {
                    _environmentDatabase = new SharedTestEnvironmentConfig();
                }
            }

            return _environmentDatabase;
        }
    }
    
    private readonly string _environmentFile;

    private SharedTestEnvironmentConfig()
    {
        var stack = new StackTrace();

        var frame = stack.GetFrame(1);

        if (frame?.GetMethod() is { DeclaringType: { Assembly: { Location: { } currentExecutable } } } &&
            new FileInfo(currentExecutable)?.Directory?.FullName is { } currentDirectory)
        {
            _environmentFile = Path.Combine(currentDirectory, "Environment.json");
        }
        else
        {
            _environmentFile = "Environment.json";
        }
        
        LoadEnvironment();
    }

    private EnvironmentData _environmentData;

    public Databases DatabaseType => map(_environmentData.DatabaseType);


    private void LoadEnvironment()
    {
        if (File.Exists(_environmentFile) && File.ReadAllText(_environmentFile) is { } contentJson)
        {
            if (JsonConvert.DeserializeObject<EnvironmentData>(contentJson) is { } environmentData)
            {
                _environmentData = environmentData;

                return;
            }
        }

        _environmentData = new EnvironmentData("sqlite");
    }

    private Databases map(string value)
    {
        value = value.ToLowerInvariant();

        if (value == "sqlite") return Databases.SqLite;
        if (value == "mysql") return Databases.MySql;
        if (value == "sqlserver") return Databases.SqlServer;
        if (value == "postgre") return Databases.Postgre;

        return Databases.None;
    }
}