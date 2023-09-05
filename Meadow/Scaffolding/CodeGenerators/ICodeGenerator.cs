using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.CodeGenerators
{
    public interface ICodeGenerator
    {
        Code Generate();
        
    }
}