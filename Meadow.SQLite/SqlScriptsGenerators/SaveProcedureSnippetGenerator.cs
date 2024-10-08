using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.SaveProcedure)]
    public class SaveProcedureSnippetGenerator : SqLiteRepetitionHandlerProcedureGeneratorBase
    {
        public SaveProcedureSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations) :
            base(construction, configurations)
        {
        }

        private record ProcedureReplacements(string ProcedureName, string Where, string InsertValues,
            string InsertColumns, string Updates);
        
        


        private readonly string _preKeyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyNoneIdParametersSet = GenerateKey();

        private readonly string _preKeyWhereClause = GenerateKey();
        private readonly string _preKeyInsertColumns = GenerateKey();
        private readonly string _preKeyInsertValues = GenerateKey();
        private readonly string _keyEntityFilterSegment = GenerateKey();


        private readonly string _preKeyUpdates = GenerateKey();


        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {

            replacementList.Add(_keyParameters,
                ParameterNameTypeJoint(ProcessedType.Parameters, ",", "@"));

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyNoneIdParametersSet,
                ParameterNameValueSetJoint(ProcessedType.NoneIdParameters, ",", "@"));

            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);

            var entityFilterSegment = entityFilterExpression.Success ? $" AND {entityFilterExpression.Value} " : "";

            replacementList.Add(_keyEntityFilterSegment, entityFilterSegment);
        }


        private string CreatePreReplacedTemplate(ProcedureReplacements replacements)
        {
            var p = RawTemplate;

            p = p.Replace(_preKeyProcedureName, replacements.ProcedureName);
            p = p.Replace(_preKeyUpdates, replacements.Updates);
            p = p.Replace(_preKeyWhereClause, replacements.Where);
            p = p.Replace(_preKeyInsertColumns, replacements.InsertColumns);
            p = p.Replace(_preKeyInsertValues, replacements.InsertValues);

            p = p.Replace(_preKeyProcedureName, replacements.ProcedureName);

            return p + LineTemplate;
        }

        private List<ProcedureReplacements> CreateProcedureReplacements()
        {
            var replacements = new List<ProcedureReplacements>();

            var collectivesByName = ProcessedType.RecordIdentificationProfile.CollectiveIdentifiersByName;
            var singularsByName = ProcessedType.RecordIdentificationProfile.SingularIdentifiersByName;

            foreach (var collective in collectivesByName)
            {
                var r = CreateProcedureReplacements(collective.Key, collective.Value.ToArray());

                replacements.Add(r);
            }

            foreach (var singular in singularsByName)
            {
                var r = CreateProcedureReplacements(singular.Key, singular.Value);
                
                replacements.Add(r);
            }

            return replacements;
        }


        private ProcedureReplacements CreateProcedureReplacements(string collectionName,
            params FieldKey[] identifierFields)
        {

            var analysis = ComponentsProcessor.CreateSaveProcedureAnalysis(collectionName, identifierFields);

            var whereClause = ParameterNameValueSetJoint(analysis.WhereEqualities, " AND ", "@");

            var insertColumns = string.Join(',', analysis.InsertUpdateParameters.Select(p => p.Name));

            var insertValues = string.Join(',', analysis.InsertUpdateParameters.Select(p => "@" + p.Name));

            var updateParameters = ParameterNameValueSetJoint(analysis.InsertUpdateParameters, ",", "@");

            return new ProcedureReplacements(
                ProcessedType.SaveProcedureNames[collectionName],
                whereClause, insertValues, insertColumns, updateParameters);
        }
        
        
        

        private const string LineTemplate =
            "\n-- ---------------------------------------------------------------------------------------------------------------------\n";


        protected string RawTemplate => $@"
{KeyHeaderCreation} {_preKeyProcedureName} ({_keyParameters}) AS

    BEGIN;
    PRAGMA temp_store = 2;
    CREATE TEMP TABLE _alreadyExists(Id INTEGER PRIMARY KEY);
    INSERT INTO _alreadyExists (Id) SELECT (1) WHERE EXISTS(SELECT * FROM {_keyTableName} WHERE {_preKeyWhereClause}{_keyEntityFilterSegment});

    UPDATE {_keyTableName} SET {_preKeyUpdates} WHERE {_preKeyWhereClause}{_keyEntityFilterSegment} AND EXISTS (SELECT * FROM _alreadyExists);

    INSERT INTO {_keyTableName} ({_preKeyInsertColumns}) SELECT {_preKeyInsertValues} WHERE NOT EXISTS (SELECT * FROM _alreadyExists);

    SELECT * FROM {_keyTableName} WHERE (({_preKeyWhereClause}) OR ROWID = LAST_INSERT_ROWID()) {_keyEntityFilterSegment} LIMIT 1;
    DROP TABLE _alreadyExists;
    END;
GO
".Trim();

        protected override string Template =>
            string.Join('\n', CreateProcedureReplacements().Select(CreatePreReplacedTemplate));
    }
}