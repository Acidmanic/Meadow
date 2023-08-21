using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class CrudMacro : BuiltinMacroBase
{
    public override string Name { get; } = "Crud";


    protected override Dictionary<CommonSnippets, SnippetInstantiationInstruction> GetAssemblyBehavior()
    {
        return new Dictionary<CommonSnippets, SnippetInstantiationInstruction>
        {
            { CommonSnippets.CreateTable,  CodeGenerateBehavior.UseIdAgnostic },
            { CommonSnippets.InsertProcedure, CodeGenerateBehavior.UseIdAgnostic },
            { CommonSnippets.ReadProcedure, CodeGenerateBehavior.UseIdAware },
            { CommonSnippets.DeleteProcedure, CodeGenerateBehavior.UseById | CodeGenerateBehavior.UseAll },
            { CommonSnippets.UpdateProcedure, CodeGenerateBehavior.UseIdAgnostic },
            { CommonSnippets.SaveProcedure, CodeGenerateBehavior.UseIdAgnostic }
        };
    }
}