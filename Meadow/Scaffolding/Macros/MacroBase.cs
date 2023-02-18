using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Results;
using Meadow.Extensions;

namespace Meadow.Scaffolding.Macros;

public abstract class MacroBase : IMacro
{
    public abstract string Name { get; }


    public abstract string GenerateCode(params string[] arguments);

    public List<Assembly> LoadedAssemblies { get; set; }


    protected Type GrabTypeArgument(string[] arguments, int index)
    {
        if (index >= arguments.Length)
        {
            throw new ArgumentException($"'{Name}' macro, expects type argument which is not provided.");
        }

        var allTypes = LoadedAssemblies.ListAllAvailableClasses();

        var type = allTypes.FirstOrDefault(t => t.FullName == arguments[index]);

        if (type == null)
        {
            throw new ArgumentException($"Type {arguments[index]}, could not be found.");
        }

        return type;
    }
}