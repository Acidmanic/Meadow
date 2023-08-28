using System.Collections.Generic;
using System.Reflection;
using Meadow.Configuration;

namespace Meadow.Scaffolding.Macros;

public class LineMacro:IMacro
{
    public string Name { get; } = "Line";
    public string GenerateCode(params string[] arguments)
    {
        return
            "-- -------------------------------------------------------" +
            "--------------------------------------------------------------";
    }

    public List<Assembly> LoadedAssemblies { get; set; }
    public MeadowConfiguration Configuration { get; set; }
}