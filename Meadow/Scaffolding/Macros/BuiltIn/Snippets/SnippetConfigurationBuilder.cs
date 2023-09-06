using System;
using Acidmanic.Utilities.Filtering.Utilities;
using Acidmanic.Utilities.Results;
using Meadow.Extensions;

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

    public SnippetConfigurationBuilder Behavior(IdAwarenessBehavior behavior)
    {
        _configurations.IdAwarenessBehavior = behavior;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseIdAware()
    {
        _configurations.IdAwarenessBehavior = IdAwarenessBehavior.UseIdAware;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseAll()
    {
        _configurations.IdAwarenessBehavior = IdAwarenessBehavior.UseAll;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseNone()
    {
        _configurations.IdAwarenessBehavior = IdAwarenessBehavior.UseNone;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseById()
    {
        _configurations.IdAwarenessBehavior = IdAwarenessBehavior.UseById;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseEveryMethod()
    {
        _configurations.IdAwarenessBehavior = IdAwarenessBehavior.UseEveryMethod;

        return this;
    }

    public SnippetConfigurationBuilder BehaviorUseIdAgnostic()
    {
        _configurations.IdAwarenessBehavior = IdAwarenessBehavior.UseIdAgnostic;

        return this;
    }


    public SnippetConfigurationBuilder OverrideEntityType(Type type)
    {
        _configurations.OverrideEntityType = new Result<Func<SnippetConstruction, Type>>(true, c => type);

        return this;
    }


    public SnippetConfigurationBuilder OverrideEntityTypeByFilterResults()
    {
        _configurations.OverrideEntityType = new Result<Func<SnippetConstruction, Type>>(true,
            c => FilteringTypeUtilities.GetFilterResultsType(c.EntityType));

        return this;
    }

    public SnippetConfigurationBuilder OverrideEntityTypeBySearchIndex()
    {
        _configurations.OverrideEntityType = new Result<Func<SnippetConstruction, Type>>(true,
            c => FilteringTypeUtilities.GetSearchIndexType(c.EntityType));

        return this;
    }

    public SnippetConfigurationBuilder OverrideEntityType<TEntity>()
    {
        _configurations.OverrideEntityType = new Result<Func<SnippetConstruction, Type>>(true, t => typeof(TEntity));

        return this;
    }

    public SnippetConfigurationBuilder OverrideEntityType(Func<SnippetConstruction, Type> typeManipulator)
    {
        _configurations.OverrideEntityType = new Result<Func<SnippetConstruction, Type>>(true, typeManipulator);

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


    public SnippetConfigurationBuilder OverrideDbObjectName(string name)
    {
        _configurations.OverrideDbObjectName = new Result<Func<SnippetConstruction, string>>
            (true, c => name);
        return this;
    }

    public SnippetConfigurationBuilder OverrideDbObjectName(Func<string> nameFactory)
    {
        _configurations.OverrideDbObjectName = new Result<Func<SnippetConstruction, string>>
            (true, c => nameFactory());
        return this;
    }

    public SnippetConfigurationBuilder OverrideDbObjectName(Func<SnippetConstruction, string> nameFactory)
    {
        _configurations.OverrideDbObjectName = new Result<Func<SnippetConstruction, string>>(true, nameFactory);
        return this;
    }

    public SnippetConfigurationBuilder OverrideDbObjectNameToFilterResultsTableName()
    {
        _configurations.OverrideDbObjectName = new Result<Func<SnippetConstruction, string>>
            (true, c => c.MeadowConfiguration.GetNameConvention(c.EntityType).FilterResultsTableName);
        return this;
    }

    public SnippetConfigurationBuilder OverrideDbObjectNameToSearchIndexTableName()
    {
        _configurations.OverrideDbObjectName = new Result<Func<SnippetConstruction, string>>
            (true, c => c.MeadowConfiguration.GetNameConvention(c.EntityType).SearchIndexTableName);
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