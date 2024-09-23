using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Filtering;
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

    public string ParameterNameTypePair(IEnumerable<Parameter> parameters, string delimiter,ParameterUsage usage)=> string.Empty;

    public string ParameterNameValueSetPair(IEnumerable<Parameter> parameters, string delimiter)=> string.Empty;

    public string ParameterNameTypePair(Parameter p,ParameterUsage usage)=> string.Empty;
    

    public string ParameterNameValueSetPair(Parameter p)=> string.Empty;
    public ISnippetToolbox CloneFor(Type entityType) => this;
}