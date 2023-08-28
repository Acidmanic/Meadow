using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadProcedureGenerator : SqLiteByTemplateProcedureGeneratorBase
    {
        public bool ById { get; }


        public ReadProcedureGenerator(Type type, MeadowConfiguration configuration, bool byId)
            : base(type, configuration)
        {
            ById = byId;
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParametersDeclaration = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName,
                ById
                    ? ProcessedType.NameConvention.SelectByIdProcedureName
                    : ProcessedType.NameConvention.SelectAllProcedureName);

            replacementList.Add(_keyParametersDeclaration,
                ById ? $"({ParameterNameTypeJoint(ProcessedType.IdParameter, "@")})" : "");

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var whereClause =
                ById ? $" WHERE {ProcessedType.IdParameter?.Name} = @{ProcessedType.IdParameter?.Name}" : "";

            replacementList.Add(_keyWhereClause, whereClause);
        }

        protected override string Template => $@"
{KeyHeaderCreation} {_keyProcedureName}{_keyParametersDeclaration} AS
    SELECT * FROM {_keyTableName}{_keyWhereClause};
GO
".Trim();
    }
}