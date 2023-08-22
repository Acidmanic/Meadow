using System;
using Acidmanic.Utilities.Results;

namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets;

public class SnippetConfigurations
{
    public CodeGenerateBehavior CodeGenerateBehavior { get; set; }
    
    public Result<Type> OverrideEntity { get; set; }
    
    
    public RepetitionHandling RepetitionHandling { get; set; }

    
    public SnippetConfigurations( CodeGenerateBehavior codeGenerateBehavior)
    {
        CodeGenerateBehavior = codeGenerateBehavior;

        OverrideEntity = new Result<Type>().FailAndDefaultValue();
    }

    public SnippetConfigurations(CodeGenerateBehavior codeGenerateBehavior,Result<Type> overrideEntity)
    {
        CodeGenerateBehavior = codeGenerateBehavior;

        OverrideEntity = overrideEntity;
    }

    public SnippetConfigurations()
    {
        CodeGenerateBehavior = CodeGenerateBehavior.UseNone;

        OverrideEntity = new Result<Type>().FailAndDefaultValue();
    }


    public static implicit operator CodeGenerateBehavior(SnippetConfigurations value)
    {
        return value.CodeGenerateBehavior;
    }

    public static implicit operator SnippetConfigurations(CodeGenerateBehavior value)
    {
        return new SnippetConfigurations(value);
    }
    
    
    
}