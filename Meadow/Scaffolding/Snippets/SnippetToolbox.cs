using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection.Extensions;
using Meadow.Contracts;
using Meadow.DataAccessResolving;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators.CodeGeneratingComponents;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Utility;

namespace Meadow.Scaffolding.Snippets;

public class SnippetToolbox
{
    public ProcessedType ProcessedType { get; }
        
    public  ComponentsProcessor ComponentsProcessor { get; }

    public  Type EntityType { get; }

    public  Type EntityTypeOrOverridenEntityType { get; }

    public  FilterQuery RegisteredFilter { get; }
    

    public  Type EffectiveType => EntityTypeOrOverridenEntityType.GetAlteredOrOriginal();
    
    public  SnippetConstruction Construction { get; }
    
    public  SnippetConfigurations Configurations { get; }
    
    public  IDbTypeNameMapper TypeNameMapper { get; }
        
    public  ISqlExpressionTranslator SqlExpressionTranslator { get; }
    
    public  DataAccessServiceResolver DataAccessServiceResolver { get; }
    
    public SnippetToolbox(SnippetConstruction construction, SnippetConfigurations configurations)
    {
        Construction = construction;
        
        Configurations = configurations;

        DataAccessServiceResolver = new DataAccessServiceResolver(construction.MeadowConfiguration);

        TypeNameMapper = DataAccessServiceResolver.DbTypeNameMapper;
        
        SqlExpressionTranslator = DataAccessServiceResolver.SqlExpressionTranslator;
        
        EntityType = Construction.EntityType;

        EntityTypeOrOverridenEntityType = Configurations.OverrideEntityType
            ? Configurations.OverrideEntityType.Value(Construction)
            : Construction.EntityType;

        ProcessedType = EntityTypeUtilities.Process(EntityTypeOrOverridenEntityType,
            Construction.MeadowConfiguration, TypeNameMapper);

        ComponentsProcessor = new ComponentsProcessor(ProcessedType);
            
        RegisteredFilter = GetRegisteredFilter();
    }
    
    private FilterQuery GetRegisteredFilter() => GetRegisteredFilter(EffectiveType);
        
        
    private FilterQuery GetRegisteredFilter(Type type)
    {
            
        if (Construction.MeadowConfiguration.Filters.ContainsKey(type))
        {
            return Construction.MeadowConfiguration.Filters[type];
        }

        return new FilterQuery();
    }
}