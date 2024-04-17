using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Translators.Contracts.Translatabels.MeadowSetup;

public class MeadowSetupDatabaseExistsProcedureTranslatable : ITranslatable
{
    private readonly string _databaseName;
    private readonly IDbTypeNameMapper _dbTypeNameMapper;
    private readonly MeadowConfiguration _meadowConfiguration;

    private record NameShell(string DatabaseName);
    
    public MeadowSetupDatabaseExistsProcedureTranslatable(string databaseName, MeadowConfiguration meadowConfiguration, IDbTypeNameMapper dbTypeNameMapper)
    {
        _databaseName = databaseName;
        _meadowConfiguration = meadowConfiguration;
        _dbTypeNameMapper = dbTypeNameMapper;
    }

    public string Translate(int indent = 0)
    {
        
        var pt = new ProcedureTranslatable(NameConvention.Reserved.DatabaseExists,
            new ParameterDeclarationsTranslatable(_dbTypeNameMapper,_meadowConfiguration)
            {
                Inclusions = new FiledManipulationMarker(),
                InputTypes = new List<Type>(){typeof(NameShell)}
            })
        {
            ProcedureContent = new IfTranslatable()
            {
                Condition = new DatabaseExistsTranslatable(_databaseName),
                Content = new DirectStringTranslatable("SELECT 1 AS 'Result';"),
                ElseContent = new DirectStringTranslatable("SELECT 0 AS 'Result';")
            },
            RepetitionHandling = RepetitionHandling.Alter
        };

        return pt.Translate(indent);
    }
}