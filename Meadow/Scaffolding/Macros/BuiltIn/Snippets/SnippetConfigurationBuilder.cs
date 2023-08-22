using System;
using Acidmanic.Utilities.Results;

namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets;

public class SnippetConfigurationBuilder
{
    private SnippetConfigurations _configurations;


    public SnippetConfigurationBuilder()
    {
        _configurations = new SnippetConfigurations();
    }

    public SnippetConfigurationBuilder(SnippetConfigurations configurations)
    {
        _configurations = configurations;
    }

    public SnippetConfigurationBuilder Behavior(CodeGenerateBehavior behavior)
    {
        _configurations.CodeGenerateBehavior = behavior;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseIdAware()
    {
        _configurations.CodeGenerateBehavior = CodeGenerateBehavior.UseIdAware;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseAll()
    {
        _configurations.CodeGenerateBehavior = CodeGenerateBehavior.UseAll;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseNone()
    {
        _configurations.CodeGenerateBehavior = CodeGenerateBehavior.UseNone;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseById()
    {
        _configurations.CodeGenerateBehavior = CodeGenerateBehavior.UseById;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseEveryMethod()
    {
        _configurations.CodeGenerateBehavior = CodeGenerateBehavior.UseEveryMethod;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseIdAgnostic()
    {
        _configurations.CodeGenerateBehavior = CodeGenerateBehavior.UseIdAgnostic;

        return this;
    }


    public SnippetConfigurationBuilder OverrideEntityType(Type type)
    {
        _configurations.OverrideEntity = new Result<Type>(true, type);

        return this;
    }

    public SnippetConfigurationBuilder OverrideEntityType<TEntity>()
    {
        _configurations.OverrideEntity = new Result<Type>(true, typeof(TEntity));

        return this;
    }

    public SnippetConfigurationBuilder RepetitionHandling(RepetitionHandling handling)
    {
        _configurations.RepetitionHandling = handling;

        return this;
    }

    public SnippetConfigurationBuilder OnRepetitionCreateAgain()
    {
        _configurations.RepetitionHandling = Snippets.RepetitionHandling.Create;

        return this;
    }

    public SnippetConfigurationBuilder OnRepetitionAlterPrevious()
    {
        _configurations.RepetitionHandling = Snippets.RepetitionHandling.Alter;

        return this;
    }

    public SnippetConfigurationBuilder OnRepetitionSkipNewOne()
    {
        _configurations.RepetitionHandling = Snippets.RepetitionHandling.Skip;

        return this;
    }


    public SnippetConfigurations Build()
    {
        var configurations = _configurations;

        Clear();

        return configurations;
    }

    public void Clear()
    {
        _configurations = new SnippetConfigurations();
    }
}