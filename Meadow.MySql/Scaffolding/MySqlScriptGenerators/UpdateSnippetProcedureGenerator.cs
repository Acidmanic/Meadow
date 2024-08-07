using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class UpdateSnippetProcedureGenerator<TEntity> : UpdateSnippetProcedureGenerator
    {
        public UpdateSnippetProcedureGenerator(MeadowConfiguration configuration)
            : base(new SnippetConstruction
            {
                EntityType = typeof(TEntity),
                MeadowConfiguration = configuration
            }, SnippetConfigurations.Default())
        {
        }
    }

    [CommonSnippet(CommonSnippets.UpdateProcedure)]
    public class UpdateSnippetProcedureGenerator : MySqlRepetitionHandlerProcedureGeneratorBase
    {
        public UpdateSnippetProcedureGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations) : base(construction, configurations)
        {
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keySetClause = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyEntityFilterSegment = GenerateKey();


        protected override string GetProcedureName(bool fullTree)
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.UpdateProcedureName);
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            if (!ProcessedType.HasId)
            {
                return;
            }
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var parameters = string.Join(',', ProcessedType.Parameters.Select(p => ParameterNameTypeJoint(p, "IN ")));

            replacementList.Add(_keyParameters, parameters);

            var setClause = string.Join(',', ProcessedType.NoneIdParameters.Select(p => p.Name + "=" + p.Name));

            replacementList.Add(_keySetClause, setClause);

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);
            
            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);
            
            var entityFilterSegment = entityFilterExpression.Success ? $" AND ({entityFilterExpression.Value}) " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);
        }

        protected override string Template => ProcessedType.HasId? $@"
{KeyCreationHeader} {KeyProcedureName}({_keyParameters})
BEGIN
    UPDATE {_keyTableName} SET {_keySetClause} WHERE {_keyTableName}.{_keyIdFieldName}={_keyIdFieldName}{_keyEntityFilterSegment};
    SELECT * FROM {_keyTableName} WHERE {_keyTableName}.{_keyIdFieldName}={_keyIdFieldName}{_keyEntityFilterSegment};
END;
".Trim():"";
    }
}