using System;
using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Results;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class FilterResultsTableMacro:BuiltinMacroBase
{
    public override string Name => "FilterResultsTable";

    protected override bool TakesTypeArgument => false;

    protected override Dictionary<CommonSnippets, SnippetInstantiationInstruction> GetAssemblyBehavior()
    {
        return new Dictionary<CommonSnippets, SnippetInstantiationInstruction>
        {
            { CommonSnippets.CreateTable ,new SnippetInstantiationInstruction(
                CodeGenerateBehavior.UseIdAgnostic,new Result<Type>(true,typeof(FilterResult)))}
        };
    }
}