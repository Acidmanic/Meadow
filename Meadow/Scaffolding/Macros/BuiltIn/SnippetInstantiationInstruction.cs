using System;
using Acidmanic.Utilities.Results;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class SnippetInstantiationInstruction
{
    public CodeGenerateBehavior CodeGenerateBehavior { get; set; }
    
    public Result<Type> OverrideEntity { get; set; }

    public SnippetInstantiationInstruction( CodeGenerateBehavior codeGenerateBehavior)
    {
        CodeGenerateBehavior = codeGenerateBehavior;

        OverrideEntity = new Result<Type>().FailAndDefaultValue();
    }

    public SnippetInstantiationInstruction(CodeGenerateBehavior codeGenerateBehavior,Result<Type> overrideEntity)
    {
        CodeGenerateBehavior = codeGenerateBehavior;

        OverrideEntity = overrideEntity;
    }

    public SnippetInstantiationInstruction()
    {
        CodeGenerateBehavior = CodeGenerateBehavior.UseNone;

        OverrideEntity = new Result<Type>().FailAndDefaultValue();
    }


    public static implicit operator CodeGenerateBehavior(SnippetInstantiationInstruction value)
    {
        return value.CodeGenerateBehavior;
    }

    public static implicit operator SnippetInstantiationInstruction(CodeGenerateBehavior value)
    {
        return new SnippetInstantiationInstruction(value);
    }
}