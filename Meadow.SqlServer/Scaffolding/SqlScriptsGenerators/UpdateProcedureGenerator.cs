using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class UpdateProcedureGenerator<TEntity> : UpdateProcedureGenerator
    {
        public UpdateProcedureGenerator(MeadowConfiguration configuration)
            : base(typeof(TEntity), configuration)
        {
        }
    }

    [CommonSnippet(CommonSnippets.UpdateProcedure)]
    public class UpdateProcedureGenerator : SqlServerByTemplateCodeGeneratorBase
    {
        public UpdateProcedureGenerator(Type type, MeadowConfiguration configuration) : base(type, configuration)
        {
        }

        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keySetValues = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();

        protected override string GetProcedureName(bool fullTree)
        {
            return IsDatabaseObjectNameForced
                ? ForcedDatabaseObjectName
                : ProcessedType.NameConvention.UpdateProcedureName;
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
        }

        protected override string Template => $@"
{KeyCreationHeader} {KeyProcedureName}({_keyParameters}) AS
    UPDATE {_keyTableName}
    SET {_keySetValues}
    WHERE {_keyIdFieldName}=@{_keyIdFieldName};
    
    SELECT * FROM {_keyTableName} WHERE {_keyIdFieldName}=@{_keyIdFieldName};
GO
".Trim();
    }
}