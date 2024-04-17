using System.Collections.Generic;
using System.Reflection;
using Meadow.Configuration;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class MeadowEssentialsMacro:IMacro
{
    public string Name => "Meadow-Essentials";
    public string GenerateCode(params string[] arguments)
    {
        throw new System.NotImplementedException();
    }

    public List<Assembly> LoadedAssemblies { get; set; } = new List<Assembly>();
    public MeadowConfiguration Configuration { get; set; } = new MeadowConfiguration();
}