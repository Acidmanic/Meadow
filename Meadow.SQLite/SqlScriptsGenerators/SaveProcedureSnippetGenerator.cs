using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
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


        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, GetProcedureName());

            replacementList.Add(_keyParameters,
                ParameterNameTypeJoint(ProcessedType.Parameters, ",", "@"));

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyNoneIdParametersSet,
                ParameterNameValueSetJoint(ProcessedType.NoneIdParameters, ",", "@"));


            var insertParameters = (ProcessedType.HasId && ProcessedType.IdField.IsAutoValued)
                ? ProcessedType.NoneIdParameters
                : ProcessedType.Parameters;
            
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

            
        }

        private string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.SaveProcedureName);
        }
        
        protected override string Template => $@"
{KeyHeaderCreation} {_keyProcedureName} ({_keyParameters}) AS
    UPDATE {_keyTableName}  SET {_keyNoneIdParametersSet} WHERE {_keyWhereClause};
    INSERT INTO {_keyTableName} ({_keyInsertColumns}) SELECT {_keyInsertParameterValues}
        WHERE NOT EXISTS(SELECT * FROM {_keyTableName} WHERE {_keyWhereClause});
    SELECT * FROM {_keyTableName} WHERE {_keyWhereClause} OR ROWID = LAST_INSERT_ROWID() LIMIT 1;
GO
".Trim();
    }
}