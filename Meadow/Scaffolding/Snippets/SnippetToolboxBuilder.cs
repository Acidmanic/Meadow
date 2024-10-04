using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Snippets;
/// <summary>
/// This class aims to ease creating SnippetToolboxes for client codes which need to generate script out of snippets
/// outside of macros, like DataAccess cores trying to construct meadow databases and etc.
/// </summary>
public class SnippetToolboxBuilder<TEntity> :SnippetToolboxBuilder
{
    public SnippetToolboxBuilder(MeadowConfiguration configuration) : base(configuration, typeof(TEntity))
    {
    }
}
/// <summary>
/// This class aims to ease creating SnippetToolboxes for client codes which need to generate script out of snippets
/// outside of macros, like DataAccess cores trying to construct meadow databases and etc.
/// </summary>
public class SnippetToolboxBuilder
{

    private Type _entityType;
    private readonly MeadowConfiguration _meadowConfiguration;
    private readonly SnippetConfigurationBuilder _configurationBuilder;


    public SnippetToolboxBuilder(MeadowConfiguration configuration) : this(configuration, typeof(object))
    {
    }

    public SnippetToolboxBuilder(MeadowConfiguration configuration, Type entityType)
    {
        _configurationBuilder = new SnippetConfigurationBuilder();
        
        _meadowConfiguration = configuration;

        _entityType = entityType;

        Clear();
    }


    public SnippetToolboxBuilder RepetitionHandling(RepetitionHandling repetitionHandling)
    {
        _configurationBuilder.RepetitionHandling(repetitionHandling);

        return this;
    }

    public SnippetToolboxBuilder BehaviorUseById()
    {
        _configurationBuilder.BehaviorUseById();

        return this;
    }

    public SnippetToolboxBuilder OverrideDbObjectName(string name)
    {
        _configurationBuilder.OverrideDbObjectName(name);
        
        return this;
    }
    
    public SnippetToolboxBuilder BehaviorUseAll()
    {
        _configurationBuilder.BehaviorUseAll();

        return this;
    }
    
    public SnippetToolboxBuilder BehaviorUseNone()
    {
        _configurationBuilder.BehaviorUseNone();

        return this;
    }
    
    public SnippetToolboxBuilder BehaviorUseEveryMethod()
    {
        _configurationBuilder.BehaviorUseEveryMethod();

        return this;
    } 
    
    public SnippetToolboxBuilder BehaviorUseIdAware()
    {
        _configurationBuilder.BehaviorUseIdAware();

        return this;
    }
    
    public SnippetToolboxBuilder BehaviorUseIdAgnostic()
    {
        _configurationBuilder.BehaviorUseIdAgnostic();

        return this;
    }

    public SnippetToolboxBuilder EntityType(Type type)
    {
        _entityType = type;

        return this;
    }

    public SnippetToolboxBuilder Clear()
    {
        _configurationBuilder.Clear();

        return this;
    }
    
    public SnippetToolbox Build()
    {
        return new SnippetToolbox(
            new SnippetConstruction{EntityType = _entityType,MeadowConfiguration = _meadowConfiguration},
            _configurationBuilder.Build()
            );
    }
}

