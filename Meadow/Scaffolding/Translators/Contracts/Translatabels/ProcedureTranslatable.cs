using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Translators.Utilities;

namespace Meadow.Scaffolding.Translators.Contracts.Translatabels;

public class ProcedureTranslatable:ITranslatable
{

    private readonly string _procedureName;

    private readonly ITranslatable _parameterDeclarations;
    
    public ITranslatable ProcedureContent { get; set; } = Translatable.Empty;

    public RepetitionHandling RepetitionHandling { get; set; } = RepetitionHandling.Create;
    
    public ProcedureTranslatable(string procedureName,ITranslatable parameterDeclarations)
    {
        _procedureName = procedureName;

        _parameterDeclarations = parameterDeclarations;
    }


    public ProcedureTranslatable(
        string procedureName, 
        IDbTypeNameMapper typeNameMapper,
        MeadowConfiguration configuration,List<Type> types) :this(procedureName,
        new ParameterDeclarationsTranslatable(typeNameMapper,configuration)
        {
            InputTypes = types
        })
    {
        
    }
    
    public ProcedureTranslatable(
        string procedureName, 
        IDbTypeNameMapper typeNameMapper,
        MeadowConfiguration configuration,params Type[] types) :this(procedureName,
        new ParameterDeclarationsTranslatable(typeNameMapper,configuration)
        {
            InputTypes = types.ToList()
        })
    {
        
    }

    public virtual string Translate(int indent = 0)
    {
        var procedure = $"{GetCreationHeader(_procedureName,indent)} {_procedureName}{ParameterDeclarations()}\n";

        procedure += $"{S.Indent(indent)}BEGIN\n";

        procedure += ProcedureContent.Translate(indent + 1);

        procedure += $"{S.Indent(indent)}END;";
        
        return procedure;
    }
    
    protected virtual string GetCreationHeader(string procedureName,int indent)
    {
        if (RepetitionHandling == RepetitionHandling.Alter)
        {
            return $"{S.Indent(indent)}DROP PROCEDURE IF EXISTS " + procedureName + ";" +
                   $"{S.Indent(indent)}\nCREATE PROCEDURE";
        }

        return $"{S.Indent(indent)}CREATE PROCEDURE";
    }

    protected virtual bool ParametersInParentheses(bool hasParameters) => true;


    private string ParameterDeclarations()
    {
        var declaration = _parameterDeclarations.Translate();

        if (ParametersInParentheses(!string.IsNullOrWhiteSpace(declaration)))
        {
            return $"({declaration})";
        }

        return declaration;
    }
}