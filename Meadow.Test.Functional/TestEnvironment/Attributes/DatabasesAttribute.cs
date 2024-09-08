using Meadow.Test.Shared;

namespace Meadow.Test.Functional.TestEnvironment.Attributes;

public class DatabasesAttribute
{
    public DatabasesAttribute(Databases databases)
    {
        Databases = databases;
    }

    public Databases Databases { get;  }
}