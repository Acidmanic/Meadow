using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class UpdateProcedureGenerator<TEntity> : UpdateProcedureGenerator
    {
        public UpdateProcedureGenerator() : base(typeof(TEntity))
        {
        }
    }

    [CommonSnippet(CommonSnippets.UpdateProcedure)]
    public class UpdateProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        private ProcessedType ProcessedType { get; }

        public UpdateProcedureGenerator(Type type) : base(new SqlDbTypeNameMapper())
        {
            ProcessedType = Process(type);
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keySetValues = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName,
                IsDatabaseObjectNameForced
                    ? ForcedDatabaseObjectName
                    : ProcessedType.NameConvention.UpdateProcedureName);

            var parameters = string.Join(',', ProcessedType.Parameters
                .Select(p => SqlProcedureDeclaration(p, "@")));

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keySetValues, string.Join(',', ProcessedType.NoneIdParameters
                .Select(p => p.Name + " = @" + p.Name)));

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);
        }

        protected override string Template => $@"
CREATE PROCEDURE {_keyProcedureName}({_keyParameters}) AS
    UPDATE {_keyTableName}
    SET {_keySetValues}
    WHERE {_keyIdFieldName}=@{_keyIdFieldName};
    
    SELECT * FROM {_keyTableName} WHERE {_keyIdFieldName}=@{_keyIdFieldName};
GO
".Trim();
    }
}