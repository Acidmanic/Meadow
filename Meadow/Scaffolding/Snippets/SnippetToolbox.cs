using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
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


    public string GetFiltersWhereClause(ColumnNameTranslation translation, string successPrefix, string successPostfix)
    {
        var entityFilterExpression = GetFiltersWhereClause(translation);

        var entityFilterSegment = entityFilterExpression.Success ? $"{successPrefix}{entityFilterExpression.Value}{successPostfix}" : "";

        return entityFilterSegment;
    }
    
    public Result<string> GetFiltersWhereClause(ColumnNameTranslation translation) =>
        GetFiltersWhereClause(EffectiveType, translation);
        
        
    public Result<string> GetFiltersWhereClause(Type type, ColumnNameTranslation translation)
    {
        var queryFilter = GetRegisteredFilter(type);

        var filterItems = queryFilter.Items();

        var count = filterItems?.Count ?? 0;

        if (count == 0)
        {
            return new Result<string>().FailAndDefaultValue();
        }

        var translatedQuery = SqlExpressionTranslator.TranslateFilterQueryToDbExpression(queryFilter, translation);

        return new Result<string>(true, translatedQuery);
    }
    
    public string ParameterNameTypeJoint(IEnumerable<Parameter> parameters, string delimiter,
        string namePrefix = "")
    {
        return string.Join(delimiter, parameters.Select(p => ParameterNameTypeJoint(p, namePrefix)));
    }
    
    public string ParameterNameValueSetJoint(IEnumerable<Parameter> parameters, string delimiter,
        string valuePrefix = "")
    {
        return string.Join(delimiter, parameters.Select(p => ParameterNameValueSetJoint(p, valuePrefix)));
    }
    public string ParameterNameTypeJoint(Parameter p, string namePrefix = "")
    {
        return namePrefix + p.Name + " " + p.Type;
    }

    public string ParameterNameValueSetJoint(Parameter p, string valuePrefix = "")
    {
        return p.Name + " = " + valuePrefix + p.Name;
    }
}