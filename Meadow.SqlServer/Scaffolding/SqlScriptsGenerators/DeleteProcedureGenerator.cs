using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class DeleteProcedureGenerator<TEntity> : DeleteProcedureGenerator
    {
        public DeleteProcedureGenerator(bool byId) : base(typeof(TEntity), byId)
        {
        }
    }

    [CommonSnippet(CommonSnippets.DeleteProcedure)]
    public class DeleteProcedureGenerator : SqlServerByTemplateCodeGeneratorBase
    {
        public bool ById { get; }

        public DeleteProcedureGenerator(Type type, bool byId) : base(type)
        {
            ById = byId;
        }

        private readonly string _keyParametersParentheses = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();


        protected override string GetProcedureName()
        {
            if (IsDatabaseObjectNameForced)
            {
                return ForcedDatabaseObjectName;
            }

            return ById
                ? ProcessedType.NameConvention.DeleteByIdProcedureName
                : ProcessedType.NameConvention.DeleteAllProcedureName;
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var idParameter = "( @" + ProcessedType.IdParameter.Name + " "
                              + ProcessedType.IdParameter.Type + ")";

            replacementList.Add(_keyParametersParentheses, ById ? idParameter : "");

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var whereClause = ById ? $"WHERE {ProcessedType.IdParameter.Name}=@{ProcessedType.IdParameter.Name}" : "";

            replacementList.Add(_keyWhereClause, whereClause);
        }

        protected override string Template => $@"
{KeyCreationHeader} {KeyProcedureName}{_keyParametersParentheses} AS
    DECLARE @existing int = (SELECT COUNT(*) FROM {_keyTableName});
    DELETE FROM {_keyTableName} {_keyWhereClause}
    DECLARE @delta int = @existing - (SELECT COUNT(*) FROM {_keyTableName});
    IF @delta > 0 or @existing = 0
        SELECT cast(1 as bit) Success
    ELSE
        select cast(0 as bit) Success
GO
";
    }
}