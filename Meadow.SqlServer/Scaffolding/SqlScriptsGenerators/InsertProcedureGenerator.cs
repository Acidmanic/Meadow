using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{

    
    [CommonSnippet(CommonSnippets.InsertProcedure)]
    public class InsertProcedureGenerator : SqlServerRepetitionHandlerProcedureGeneratorBase
    {
        public InsertProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations) : base(construction, configurations)
        {
        }

        public InsertProcedureGenerator(Type entityType,MeadowConfiguration configuration):
            base(new SnippetConstruction
            {
                EntityType = entityType,
                MeadowConfiguration = configuration
            },SnippetConfigurations.Default())
        {
            
        }
        
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldType = GenerateKey();
        private readonly string _keyRecordPhrase = GenerateKey();


        protected override string GetProcedureName(bool fullTree)
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.InsertProcedureName);
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var parameters = ProcessedType.NoneIdParameters.Select(p => ParameterNameTypeJoint(p, "@"));
            var parametersClause =
                ProcessedType.NoneIdParameters.Count > 0 ? "(" + string.Join(',', parameters) + ")" : "";

            replacementList.Add(_keyParameters, parametersClause);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyColumns, string.Join(',', ProcessedType.NoneIdParameters.Select(p => p.Name)));

            replacementList.Add(_keyValues,
                string.Join(',', ProcessedType.NoneIdParameters.Select(p => "@" + p.Name)));

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);

            replacementList.Add(_keyIdFieldType, ProcessedType.IdParameter.Type);

            var recordItems = ProcessedType.Parameters.Select(p => p.Name + " @" + p.Name);

            var recordPhrase = string.Join(',', recordItems);

            replacementList.Add(_keyRecordPhrase, recordPhrase);
        }

        protected override string Template => ProcessedType.HasId ? ByIdTemplate : NoIdTemplate;

        private string ByIdTemplate => $@"
{KeyCreationHeader} {KeyProcedureName}{_keyParameters} AS
    
    INSERT INTO {_keyTableName} ({_keyColumns}) 
                   VALUES ({_keyValues})
    DECLARE @newId {_keyIdFieldType}=(IDENT_CURRENT('{_keyTableName}'));
    SELECT * FROM {_keyTableName} WHERE {_keyIdFieldName}=@newId;
GO
";

        private string NoIdTemplate => $@"
{KeyCreationHeader} {KeyProcedureName}{_keyParameters} AS
    
    INSERT INTO {_keyTableName} ({_keyColumns}) 
           VALUES ({_keyValues})
    SELECT {_keyRecordPhrase};
GO
";
    }
}