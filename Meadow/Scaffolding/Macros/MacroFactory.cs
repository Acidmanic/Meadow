using System;
using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Factories;
using Acidmanic.Utilities.Reflection.Extensions;

namespace Meadow.Scaffolding.Macros;

public class MacroFactory : FactoryBase<IMacro, string>
{
    public MacroFactory() : base(FactoryMatching.MatchByInstance)
    {
    }

    protected override bool MatchesByType(Type productType, string value)
    {
        throw new NotImplementedException();
    }

    protected override bool MatchesByInstance(IMacro product, string value)
    {
        var productName = product?.Name?.ToLower();

        value = value?.ToLower();

        if (value.AreEqualAsNullables(productName))
        {
            return value == productName;
        }

        return false;
    }

    protected override IMacro DefaultValue()
    {
        return NullMacro.Instance;
    }

    public void ScanAssemblies(IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            ScanAssembly(assembly);
        }
    }
}