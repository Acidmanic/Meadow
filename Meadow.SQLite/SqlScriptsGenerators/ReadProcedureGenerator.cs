using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadProcedureGeneratorFullTree : ReadProcedureGenerator
    {
        public ReadProcedureGeneratorFullTree(Type type, MeadowConfiguration configuration, bool byId) : base(type,
            configuration, byId)
        {
        }

        protected override bool DisableFullTree => false;
    }

    public class ReadProcedureGeneratorPlainOnly : ReadProcedureGenerator
    {
        public ReadProcedureGeneratorPlainOnly(Type type, MeadowConfiguration configuration, bool byId) : base(type,
            configuration, byId)
        {
        }

        protected override bool DisableFullTree => true;
    }

    public abstract class ReadProcedureGenerator : SqLiteByTemplateProcedureGeneratorBase
    {
        public bool ById { get; }

        protected abstract bool DisableFullTree { get; }

        public ReadProcedureGenerator(Type type, MeadowConfiguration configuration, bool byId)
            : base(type, configuration)
        {
            ById = byId;
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyProcedureNameFullTree = GenerateKey();


        private readonly string _keyParametersDeclaration = GenerateKey();

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeView = GenerateKey();

        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyWhereClauseFullTree = GenerateKey();

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName,
                ById
                    ? ProcessedType.NameConvention.SelectByIdProcedureName
                    : ProcessedType.NameConvention.SelectAllProcedureName);
            replacementList.Add(_keyProcedureNameFullTree,
                ById
                    ? ProcessedType.NameConvention.SelectByIdProcedureNameFullTree
                    : ProcessedType.NameConvention.SelectAllProcedureNameFullTree);


            replacementList.Add(_keyParametersDeclaration,
                ById ? $"({ParameterNameTypeJoint(ProcessedType.IdParameter, "@")})" : "");

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);
            replacementList.Add(_keyFullTreeView, ProcessedType.NameConvention.FullTreeViewName);

            var whereClause =
                ById ? $" WHERE {ProcessedType.IdParameter?.Name} = @{ProcessedType.IdParameter?.Name}" : "";
            var whereClauseFullTree =
                ById ? $" WHERE {ProcessedType.IdParameterFullTree?.Name} = @{ProcessedType.IdParameter?.Name}" : "";

            replacementList.Add(_keyWhereClause, whereClause);
            replacementList.Add(_keyWhereClauseFullTree, whereClauseFullTree);
        }


        private string PlainObjectTemplate => $@"
{KeyHeaderCreation} {_keyProcedureName}{_keyParametersDeclaration} AS
    SELECT * FROM {_keyTableName}{_keyWhereClause};
GO
".Trim();

        private string FullTreeTemplate => $@"
{KeyHeaderCreation} {_keyProcedureNameFullTree}{_keyParametersDeclaration} AS
    SELECT * FROM {_keyFullTreeView}{_keyWhereClauseFullTree};
GO
".Trim();


        protected override string Template =>
            DisableFullTree ? PlainObjectTemplate : PlainObjectTemplate + "\n" + FullTreeTemplate;
    }
}