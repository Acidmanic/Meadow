using System.Collections.Generic;
using System.Linq;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.UpdateProcedure)]
    public class UpdateProcedureGenerator : SqlServerRepetitionHandlerProcedureGeneratorBase
    {
        public UpdateProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations) : base(
            construction, configurations)
        {
        }

        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keySetValues = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyEntityFilterSegment = GenerateKey();

        protected override string GetProcedureName(bool fullTree)
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.UpdateProcedureName);
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var parameters = string.Join(',', ProcessedType.Parameters
                .Select(p => ParameterNameTypeJoint(p, "@")));

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keySetValues, string.Join(',', ProcessedType.NoneIdParameters
                .Select(p => p.Name + " = @" + p.Name)));

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);
            
            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.DataOwnerDotColumnName);
            
            var entityFilterSegment = entityFilterExpression.Success ? $" AND ({entityFilterExpression.Value})" : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);
        }

        protected override string Template => $@"
{KeyCreationHeader} {KeyProcedureName}({_keyParameters}) AS
    UPDATE {_keyTableName}
    SET {_keySetValues}
    WHERE {_keyIdFieldName}=@{_keyIdFieldName}{_keyEntityFilterSegment};
    SELECT * FROM {_keyTableName} WHERE {_keyIdFieldName}=@{_keyIdFieldName}{_keyEntityFilterSegment};
GO
".Trim();
    }
}