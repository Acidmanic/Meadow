using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Extensions;
using Meadow.Scaffolding.Models;
using Meadow.Utility;

namespace Meadow.Scaffolding.Translators.Contracts.Translatabels;

public class ParameterDeclarationsTranslatable : ITranslatable
{
    private readonly IDbTypeNameMapper _typeNameMapper;
    
    private readonly MeadowConfiguration _configuration;


    public ParameterDeclarationsTranslatable(IDbTypeNameMapper typeNameMapper, MeadowConfiguration configuration)
    {
        _typeNameMapper = typeNameMapper;
        _configuration = configuration;
    }

    public List<Type> InputTypes { get; set; } = new();

    public IFieldInclusion Inclusions { get; set; } = new FiledManipulationMarker();
    

    public virtual string Translate(int indent = 0)
    {
        var processedTypes = InputTypes
            .Select(t => EntityTypeUtilities.Process(t, _configuration, _typeNameMapper, Inclusions)).ToList();

        var parameters = processedTypes.Aggregate(pt => pt.Parameters);

        var parametersDeclaration = string.Join(ParameterDelimiter, parameters.Select(ParameterTerm));

        return parametersDeclaration;
    }

    private string ParameterTerm(Parameter p)
    {
        if (OrderTypeFirst)
        {
            return p.Type + " " + ParameterPrefix + p.Name;
        }

        return ParameterPrefix + p.Name + " " + p.Type;
    }

    protected virtual string ParameterPrefix => "IN ";

    protected virtual string ParameterDelimiter => ",";

    protected virtual bool OrderTypeFirst => false;
}