using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class DeleteProcedureGenerator<TEntity> : DeleteProcedureGenerator
    {
        public DeleteProcedureGenerator(MeadowConfiguration configuration, bool actById)
            : base(new SnippetConstruction
            {
                EntityType = typeof(TEntity),
                MeadowConfiguration = configuration
            }, SnippetConfigurations.IdAware(!actById))
        {
        }
    }

    [CommonSnippet(CommonSnippets.DeleteProcedure)]
    public class DeleteProcedureGenerator : SqlServerRepetitionHandlerProcedureGeneratorBase
    {
        public bool ActById { get; set; }

        public DeleteProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations) : base(construction, configurations)
        {
            
        }

        private readonly string _keyParametersParentheses = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();


        protected override string GetProcedureName(bool fullTree)
        {
           return ProvideDbObjectNameSupportingOverriding(() =>ActById
               ? ProcessedType.NameConvention.DeleteByIdProcedureName
               : ProcessedType.NameConvention.DeleteAllProcedureName);
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var idParameter = "( @" + ProcessedType.IdParameter.Name + " "
                              + ProcessedType.IdParameter.Type + ")";

            replacementList.Add(_keyParametersParentheses, ActById ? idParameter : "");

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var whereClause = ActById ? $"WHERE {ProcessedType.IdParameter.Name}=@{ProcessedType.IdParameter.Name}" : "";

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