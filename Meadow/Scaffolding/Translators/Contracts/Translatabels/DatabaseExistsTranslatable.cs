namespace Meadow.Scaffolding.Translators.Contracts.Translatabels;

public class DatabaseExistsTranslatable:ITranslatable
{

    private readonly string _databaseName;

    public DatabaseExistsTranslatable(string databaseName)
    {
        _databaseName = databaseName;
    }

    public string Translate(int indent = 0)
    {
        return $"EXISTS(SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = {_databaseName})";
    }
}