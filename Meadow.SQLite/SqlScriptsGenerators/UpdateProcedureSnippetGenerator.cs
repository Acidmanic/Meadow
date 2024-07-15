using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.UpdateProcedure)]
    public class UpdateProcedureSnippetGenerator : SqLiteRepetitionHandlerProcedureGeneratorBase
    {
        public UpdateProcedureSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations) :
            base(construction, configurations)
        {
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyNoneIdParametersSet = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyEntityFilterSegment = GenerateKey();

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, GetProcedureName());

            replacementList.Add(_keyParameters, ParameterNameTypeJoint(ProcessedType.Parameters, ",", "@"));

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyNoneIdParametersSet,
                ParameterNameValueSetJoint(ProcessedType.NoneIdParameters, ",", "@"));

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);
            
            var entityFilterExpression = GetFiltersWhereClause(false);
            
            var entityFilterSegment = entityFilterExpression.Success ? $" AND {entityFilterExpression.Value} " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);
        }

        private string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.UpdateProcedureName);
        }
        
        protected override string Template => $@"
{KeyHeaderCreation} {_keyProcedureName} ({_keyParameters}) AS

    UPDATE {_keyTableName} SET {_keyNoneIdParametersSet} WHERE {_keyTableName}.{_keyIdFieldName}=@{_keyIdFieldName};
    SELECT * FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdFieldName}=@{_keyIdFieldName}{_keyEntityFilterSegment};
GO
".Trim();
    }
}