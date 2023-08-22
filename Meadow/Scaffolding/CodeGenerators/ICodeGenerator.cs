using System;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.CodeGenerators
{
    public interface ICodeGenerator
    {
        
        RepetitionHandling RepetitionHandling { get; set; }
        
        Code Generate();
    }
}