using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class ReadProcedureGenerator<TEntity> : ReadProcedureGenerator
    {
        public ReadProcedureGenerator(MeadowConfiguration configuration, bool byId, bool appendSplit)
            : base(typeof(TEntity), configuration, byId)
        {
        }
    }

    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadProcedureGenerator : SqlServerByTemplateCodeGeneratorBase
    {
        public bool ById { get; }


        public ReadProcedureGenerator(Type type, MeadowConfiguration configuration, bool byId)
            : base(type, configuration)
        {
            ById = byId;
        }

        protected override string GetProcedureName()
        {
            if (IsDatabaseObjectNameForced)
            {
                return ForcedDatabaseObjectName;
            }

            return ById
                ? ProcessedType.NameConvention.SelectByIdProcedureName
                : ProcessedType.NameConvention.SelectAllProcedureName;
        }

        private readonly string _keyIdParameterDeclaration = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();


        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var parameterDeclaration =
                ById ? $"(@{ProcessedType.IdParameter.Name} {ProcessedType.IdParameter.Type})" : "";

            replacementList.Add(_keyIdParameterDeclaration, parameterDeclaration);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var whereClause = ById
                ? $" WHERE {ProcessedType.NameConvention.TableName}.{ProcessedType.IdParameter.Name}" +
                  $" = @{ProcessedType.IdParameter.Name}"
                : "";

            replacementList.Add(_keyWhereClause, whereClause);
        }

        protected override string Template => $@"
{KeyCreationHeader} {KeyProcedureName}{_keyIdParameterDeclaration} AS
    SELECT * FROM {_keyTableName}{_keyWhereClause};
GO".Trim();
    }
}