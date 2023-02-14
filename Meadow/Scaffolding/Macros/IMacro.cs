using System.Collections.Generic;
using System.Reflection;

namespace Meadow.Scaffolding.Macros;

public interface IMacro
{


    public string Name { get; }

    string GenerateCode(params string[] arguments);
    
    public List<Assembly> LoadedAssemblies { get; set; }
}