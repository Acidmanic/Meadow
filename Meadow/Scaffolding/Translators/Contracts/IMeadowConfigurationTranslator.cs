namespace Meadow.Scaffolding.Translators.Contracts;

public interface IMeadowConfigurationTranslator
{
    /// <summary>
    /// Must create a database with the given name
    /// </summary>
    /// <param name="databaseName">The Name Of Database to be created</param>
    /// <returns>Nothing</returns>
    string CreateDatabase(string databaseName);

    /// <summary>
    /// Must return 0/1 or false/ (depending on database system) indicating the existence of a database with the given name
    /// </summary>
    /// <param name="databaseName">Database name</param>
    /// <returns>boolean convertible value with the name Result
    /// <code>{ bool Result {get;set;}}</code>
    /// </returns>
    string DatabaseExists(string databaseName);
}