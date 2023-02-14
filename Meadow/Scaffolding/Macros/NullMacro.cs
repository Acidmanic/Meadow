using System.Collections.Generic;
using System.Reflection;

namespace Meadow.Scaffolding.Macros;

public class NullMacro : IMacro
{
    private NullMacro()
    {
        Name = "NullMacro";
    }

    public static readonly NullMacro Instance = new NullMacro();


    public string Name { get; }

    public string GenerateCode(params string[] arguments)
    {
        return "";
    }

    public List<Assembly> LoadedAssemblies { get; set; } = new List<Assembly>();
}