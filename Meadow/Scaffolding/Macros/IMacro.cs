namespace Meadow.Scaffolding.Macros;

public interface IMacro
{


    public string Name { get; }

    string GenerateCode(params string[] arguments);
}