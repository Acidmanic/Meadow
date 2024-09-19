using System;
using System.Collections.Generic;
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

namespace Meadow.Scaffolding.Snippets;

public interface ISnippetToolbox
{
    public static SnippetToolbox Null { get; } = new SnippetToolboxBuilder<object>(new MeadowConfiguration()).Build();

    ProcessedType ProcessedType { get; }

    ComponentsProcessor ComponentsProcessor { get; }

    Type EntityType { get; }

    Type EntityTypeOrOverridenEntityType { get; }

    FilterQuery RegisteredFilter { get; }

    Type EffectiveType => EntityTypeOrOverridenEntityType.GetAlteredOrOriginal();

    SnippetConstruction Construction { get; }

    SnippetConfigurations Configurations { get; }

    IDbTypeNameMapper TypeNameMapper { get; }

    ISqlTranslator SqlTranslator { get; }
    
    IValueTranslator ValueTranslator { get; }

    IDataAccessServiceResolver DataAccessServiceResolver { get; }

    FullTreeTranslation FullTreeTranslation { get; }

    string GetEntityFiltersWhereClause(string successPrefix, string successPostfix);
    
    Result<string> GetEntityFiltersWhereClause();

    Result<string> GetEntityFiltersWhereClause(Type type);

    string ParameterNameTypePair(IEnumerable<Parameter> parameters, string delimiter, ParameterUsage usage);
    
    string ParameterNameTypePair(Parameter p, ParameterUsage usage);

    string ParameterNameValueSetPair(IEnumerable<Parameter> parameters, string delimiter);


    string ParameterNameValueSetPair(Parameter p);
}