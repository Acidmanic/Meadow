namespace Meadow.Test.Functional.TestEnvironment;

public interface IDatabaseSelector
{

    void UseSqLite();
    void UseMySql();
    void UsePostgre();
    void UseSqlServer();
}