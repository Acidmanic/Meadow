using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Results;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessResolving;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators.CodeGeneratingComponents;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Sql;

namespace Meadow.Scaffolding.Snippets;

public class NullSnippetToolbox:ISnippetToolbox
{
    public ProcessedType ProcessedType { get; } = ProcessedType.Null;

    public ComponentsProcessor ComponentsProcessor { get; } = new(ProcessedType.Null);

    public Type EntityType => ProcessedType.NameConvention.EntityType;
    
    public Type EntityTypeOrOverridenEntityType => ProcessedType.NameConvention.EntityType;

    public FilterQuery RegisteredFilter { get; } = new();

    public SnippetConstruction Construction { get; } = SnippetConstruction.Null;
    
    public SnippetConfigurations Configurations { get; } = SnippetConfigurations.Default();
    
    public IDbTypeNameMapper TypeNameMapper { get; }  =IDbTypeNameMapper.Null;
    
    public ISqlTranslator SqlTranslator { get; } = ISqlTranslator.Null;
    
    public IValueTranslator ValueTranslator { get; } = IValueTranslator.Null;

    public IDataAccessServiceResolver DataAccessServiceResolver { get; } = IDataAccessServiceResolver.Null;

    public FullTreeTranslation FullTreeTranslation { get; } =
        new FullTreeTranslation(MeadowConfiguration.Null, ProcessedType.Null, ISqlTranslator.Null);

    public string GetEntityFiltersWhereClause(string successPrefix, string successPostfix) => string.Empty;

    public Result<string> GetEntityFiltersWhereClause()=> new Result<string>().FailAndDefaultValue();

    public Result<string> GetEntityFiltersWhereClause(Type type) => new Result<string>().FailAndDefaultValue();

    public string ParameterNameTypeJoint(IEnumerable<Parameter> parameters, string delimiter, string namePrefix = "")=> string.Empty;

    public string ParameterNameValueSetJoint(IEnumerable<Parameter> parameters, string delimiter, string valuePrefix = "")=> string.Empty;

    public string ParameterNameTypeJoint(Parameter p, string namePrefix = "")=> string.Empty;

    public string ParameterNameValueSetJoint(Parameter p, string valuePrefix = "")=> string.Empty;
}