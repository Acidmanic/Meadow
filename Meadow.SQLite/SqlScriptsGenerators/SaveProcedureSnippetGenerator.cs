using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.SaveProcedure)]
    public class SaveProcedureSnippetGenerator : SqLiteRepetitionHandlerProcedureGeneratorBase
    {
        public SaveProcedureSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations) : base(construction, configurations)
        {
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyNoneIdParametersSet = GenerateKey();
        private readonly string _keyInsertColumns = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyInsertParameterValues = GenerateKey();
        private readonly string _keyEntityFilterSegment = GenerateKey();


        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, GetProcedureName());

            replacementList.Add(_keyParameters,
                ParameterNameTypeJoint(ProcessedType.Parameters, ",", "@"));

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyNoneIdParametersSet,
                ParameterNameValueSetJoint(ProcessedType.NoneIdParameters, ",", "@"));
            
            var insertParameters = ProcessedType.GetInsertParameters();
            
            replacementList.Add(_keyInsertColumns,
                string.Join(',', insertParameters.Select(p => p.Name)));

            replacementList.Add(_keyInsertParameterValues,
                string.Join(',', insertParameters.Select(p => "@" + p.Name)));
            
            var whereClause = ParameterNameValueSetJoint(ProcessedType.NoneIdUniqueParameters, " AND ", "@");

            if (ProcessedType.NoneIdUniqueParameters.Count == 0)
            {
                whereClause = ParameterNameValueSetJoint(ProcessedType.IdParameter, "@");
            }

            replacementList.Add(_keyWhereClause, whereClause);

            var entityFilterExpression = GetFiltersWhereClause(false);
            
            var entityFilterSegment = entityFilterExpression.Success ? $" AND {entityFilterExpression.Value} " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);
        }

        private string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.SaveProcedureName);
        }
        
        protected override string Template => $@"
{KeyHeaderCreation} {_keyProcedureName} ({_keyParameters}) AS

    BEGIN;
    PRAGMA temp_store = 2;
    CREATE TEMP TABLE _alreadyExists(Id INTEGER PRIMARY KEY);
    INSERT INTO _alreadyExists (Id) SELECT (1) WHERE EXISTS(SELECT * FROM {_keyTableName} WHERE {_keyWhereClause}{_keyEntityFilterSegment});

    UPDATE {_keyTableName} SET {_keyNoneIdParametersSet} WHERE {_keyWhereClause}{_keyEntityFilterSegment} AND EXISTS (SELECT * FROM _alreadyExists);

    INSERT INTO {_keyTableName} ({_keyInsertColumns}) SELECT {_keyInsertParameterValues} WHERE NOT EXISTS (SELECT * FROM _alreadyExists);

    SELECT * FROM {_keyTableName} WHERE ({_keyWhereClause} OR ROWID = LAST_INSERT_ROWID()) {_keyEntityFilterSegment} LIMIT 1;
    DROP TABLE _alreadyExists;
    END;
GO
".Trim();
    }
}