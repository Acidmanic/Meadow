using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class ReadSequenceProcedureSnippetGenerator : SqLiteRepetitionHandlerProcedureGeneratorBase
    {
        public bool ActById { get; }

        public int Top { get; }

        public bool OrderAscending { get; }


        public ReadSequenceProcedureSnippetGenerator(Type type, MeadowConfiguration configuration,
            bool actById, int top, bool orderAscending) : base(new SnippetConstruction
        {
            EntityType = type,
            MeadowConfiguration = configuration
        }, SnippetConfigurations.IdAware(!actById))
        {
            ActById = actById;
            Top = top;
            OrderAscending = orderAscending;
        }


        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParametersDeclaration = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyOrderClause = GenerateKey();
        private readonly string _keyTopClause = GenerateKey();

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, GetProcedureName());

            replacementList.Add(_keyParametersDeclaration,
                ActById ? $"({ParameterNameTypeJoint(ProcessedType.IdParameter, "@")})" : "");

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var whereClause =
                ActById ? $" WHERE {ProcessedType.IdParameter?.Name} = @{ProcessedType.IdParameter?.Name}" : "";

            replacementList.Add(_keyWhereClause, whereClause);

            replacementList.Add(_keyOrderClause, OrderAscending ? " ORDER BY ROWID ASC" : " ORDER BY ROWID DESC");

            replacementList.Add(_keyTopClause, Top > 0 ? $" LIMIT {Top}" : "");
        }

        private string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ActById
                ? ProcessedType.NameConvention.SelectByIdProcedureName
                : ProcessedType.NameConvention.SelectAllProcedureName);
        }

        protected override string Template => $@"
{KeyHeaderCreation} {_keyProcedureName}{_keyParametersDeclaration} AS
    SELECT * FROM {_keyTableName}{_keyWhereClause}{_keyOrderClause}{_keyTopClause}
GO
".Trim();
    }
}