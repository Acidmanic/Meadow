using System;
using System.Collections.Generic;

namespace Meadow.Scaffolding.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BuiltinMacroAdaptableAttribute:Attribute
{
    public BuiltinMacroAdaptableAttribute(bool idAware,params string[] belongedMacros)
    {
        IdAware = idAware;

        BelongedMacros = new List<string>(belongedMacros);
    }


    public List<string> BelongedMacros { get; }
    
    public bool IdAware { get; }
    
    
}