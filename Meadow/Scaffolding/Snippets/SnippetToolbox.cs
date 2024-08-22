using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection.Extensions;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators.CodeGeneratingComponents;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Utility;

namespace Meadow.Scaffolding.Snippets;

public class SnippetToolbox
{
    protected ProcessedType ProcessedType { get; }
        
    protected ComponentsProcessor ComponentsProcessor { get; }

    protected Type EntityType { get; }

    protected Type EntityTypeOrOverridenEntityType { get; }

    protected FilterQuery RegisteredFilter { get; }
    

    protected Type EffectiveType => EntityTypeOrOverridenEntityType.GetAlteredOrOriginal();
    
    protected SnippetConstruction Construction { get; }
    
    protected SnippetConfigurations Configurations { get; }
    
    protected IDbTypeNameMapper TypeNameMapper { get; }
        
    protected ISqlExpressionTranslator SqlExpressionTranslator { get; }
    
    protected SnippetToolbox(
        SnippetConstruction construction,
        SnippetConfigurations configurations)
    {
        Construction = construction;
        
        Configurations = configurations;

        EntityType = Construction.EntityType;

        EntityTypeOrOverridenEntityType = Configurations.OverrideEntityType
            ? Configurations.OverrideEntityType.Value(Construction)
            : Construction.EntityType;

        // ProcessedType = EntityTypeUtilities.Process(EntityTypeOrOverridenEntityType,
        //     Construction.MeadowConfiguration, execution.TypeNameMapper);

        ComponentsProcessor = new ComponentsProcessor(ProcessedType);
            
        //RegisteredFilter = GetRegisteredFilter();
    }
}