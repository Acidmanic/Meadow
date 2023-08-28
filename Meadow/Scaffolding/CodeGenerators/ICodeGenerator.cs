using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.CodeGenerators
{
    public interface ICodeGenerator
    {
        
        RepetitionHandling RepetitionHandling { get; set; }
        
        Code Generate();
        
        /// <summary>
        /// This should be delivered to instance before making a call to Generate method.
        /// </summary>
        MeadowConfiguration Configuration { get; set; }
    }
}