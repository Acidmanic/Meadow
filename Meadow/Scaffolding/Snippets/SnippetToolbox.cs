using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessResolving;
using Meadow.DataTypeMapping;
using Meadow.Models;
using Meadow.Scaffolding.CodeGenerators.CodeGeneratingComponents;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Sql;
using Meadow.Sql.Extensions;
using Meadow.Utility;

namespace Meadow.Scaffolding.Snippets;

public class SnippetToolbox : ISnippetToolbox
{
    public static ISnippetToolbox Null { get; } = new SnippetToolboxBuilder<object>(new MeadowConfiguration()).Build();

    public ProcessedType ProcessedType { get; }

    public ComponentsProcessor ComponentsProcessor { get; }

    public Type EntityType { get; }

    public Type EntityTypeOrOverridenEntityType { get; }

    public FilterQuery RegisteredFilter { get; }


    public Type EffectiveType => EntityTypeOrOverridenEntityType.GetAlteredOrOriginal();

    public SnippetConstruction Construction { get; }

    public SnippetConfigurations Configurations { get; }

    public IDbTypeNameMapper TypeNameMapper { get; }

    public ISqlTranslator SqlTranslator { get; }

    public IDataAccessServiceResolver DataAccessServiceResolver { get; }

    public FullTreeTranslation FullTreeTranslation { get; }


    public SnippetToolbox(SnippetConstruction construction, SnippetConfigurations configurations)
    {
        Construction = construction;

        Configurations = configurations;

        DataAccessServiceResolver = new DataAccessServiceResolver(construction.MeadowConfiguration);

        TypeNameMapper = DataAccessServiceResolver.DbTypeNameMapper;

        SqlTranslator = DataAccessServiceResolver.SqlTranslator;

        EntityType = Construction.EntityType;

        EntityTypeOrOverridenEntityType = Configurations.OverrideEntityType
            ? Configurations.OverrideEntityType.Value(Construction)
            : Construction.EntityType;

        ProcessedType = EntityTypeUtilities.Process(EntityTypeOrOverridenEntityType,
            Construction.MeadowConfiguration, TypeNameMapper);

        ComponentsProcessor = new ComponentsProcessor(ProcessedType);

        RegisteredFilter = GetRegisteredFilter();

        FullTreeTranslation = new FullTreeTranslation(construction.MeadowConfiguration, ProcessedType, SqlTranslator);
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


    public string GetEntityFiltersWhereClause(string successPrefix, string successPostfix)
    {
        var entityFilterExpression = GetEntityFiltersWhereClause();

        var entityFilterSegment = entityFilterExpression.Success
            ? $"{successPrefix}{entityFilterExpression.Value}{successPostfix}"
            : "";

        return entityFilterSegment;
    }


    public Result<string> GetEntityFiltersWhereClause() => GetEntityFiltersWhereClause(EffectiveType);


    public Result<string> GetEntityFiltersWhereClause(Type type)
    {
        var queryFilter = GetRegisteredFilter(type);

        var filterItems = queryFilter.Items();

        var count = filterItems?.Count ?? 0;

        if (count == 0)
        {
            return new Result<string>().FailAndDefaultValue();
        }

        var translatedQuery =
            SqlTranslator.TranslateFilterQueryToDbExpression(queryFilter,
                SqlTranslator.EntityFilterWhereClauseColumnTranslation);

        return new Result<string>(true, translatedQuery);
    }

    public string ParameterNameTypePair(IEnumerable<Parameter> parameters, string delimiter,
        ParameterUsage usage)
    {
        return string.Join(delimiter, parameters.Select(p => ParameterNameTypePair(p, usage)));
    }

    public string ParameterNameValueSetPair(IEnumerable<Parameter> parameters, string delimiter)
        => string.Join(delimiter, parameters.Select(ParameterNameValueSetPair));

    public string ParameterNameTypePair(Parameter p, ParameterUsage usage)
    {
        var name = SqlTranslator.Decorate(p, usage);

        return name + " " + p.Type;
    }

    public string ParameterNameValueSetPair(Parameter p)
    {
        var body = SqlTranslator.Decorate(p, ParameterUsage.ProcedureBody);
        var column = SqlTranslator.Decorate(p, ParameterUsage.ColumnName);

        return column + " = " + body;
    }

    public ISnippetToolbox CloneFor(Type entityType)
    {
        var construction = new SnippetConstruction()
        {
            EntityType = entityType,
            MeadowConfiguration = this.Construction.MeadowConfiguration
        };

        var configuration = new SnippetConfigurations()
        {
            RepetitionHandling = this.Configurations.RepetitionHandling,
            IdAwarenessBehavior = this.Configurations.IdAwarenessBehavior,
            OverrideEntityType = this.Configurations.OverrideEntityType,
            OverrideDbObjectName = this.Configurations.OverrideDbObjectName
        };

        return new SnippetToolbox(construction, configuration);
    }
}