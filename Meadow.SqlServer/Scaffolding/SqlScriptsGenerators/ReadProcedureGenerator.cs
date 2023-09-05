using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class ReadProcedureGeneratorPlainOnly : ReadProcedureGenerator
    {
        public ReadProcedureGeneratorPlainOnly(Type type, MeadowConfiguration configuration, bool byId) : base(type,
            configuration, byId)
        {
        }

        protected override bool DisableFullTree => true;
    }

    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadProcedureGeneratorFullTree : ReadProcedureGenerator
    {
        public ReadProcedureGeneratorFullTree(Type type, MeadowConfiguration configuration, bool byId) : base(type,
            configuration, byId)
        {
        }

        protected override bool DisableFullTree => false;
    }


    public abstract class ReadProcedureGenerator : SqlSnippetServerByTemplateCodeGeneratorBase
    {
        public bool ById { get; }

        protected abstract bool DisableFullTree { get; }

        public ReadProcedureGenerator(Type type, MeadowConfiguration configuration, bool byId)
            : base(type, configuration)
        {
            ById = byId;
        }

        protected override string GetProcedureName(bool fullTree)
        {
            if (IsDatabaseObjectNameForced)
            {
                return ForcedDatabaseObjectName;
            }

            if (fullTree)
            {
                return ById
                    ? ProcessedType.NameConvention.SelectByIdProcedureNameFullTree
                    : ProcessedType.NameConvention.SelectAllProcedureNameFullTree;    
            }
            return ById
                ? ProcessedType.NameConvention.SelectByIdProcedureName
                : ProcessedType.NameConvention.SelectAllProcedureName;
        }

        private readonly string _keyIdParameterDeclaration = GenerateKey();
        
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeViewName = GenerateKey();
        
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyWhereClauseFullTree = GenerateKey();


        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var parameterDeclaration =
                ById ? $"(@{ProcessedType.IdParameter.Name} {ProcessedType.IdParameter.Type})" : "";

            replacementList.Add(_keyIdParameterDeclaration, parameterDeclaration);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);
            replacementList.Add(_keyFullTreeViewName, ProcessedType.NameConvention.FullTreeViewName);

            var whereClause = ById
                ? $" WHERE {ProcessedType.NameConvention.TableName}.{ProcessedType.IdParameter.Name}" +
                  $" = @{ProcessedType.IdParameter.Name}"
                : "";
            var whereClauseFullTree = ById
                ? $" WHERE {ProcessedType.NameConvention.FullTreeViewName}.{ProcessedType.IdParameterFullTree.Name}" +
                  $" = @{ProcessedType.IdParameter.Name}"
                : "";

            replacementList.Add(_keyWhereClause, whereClause);
            replacementList.Add(_keyWhereClauseFullTree, whereClauseFullTree);
        }

        protected string PlainObjectTemplate => $@"
{KeyCreationHeader} {KeyProcedureName}{_keyIdParameterDeclaration} AS
    SELECT * FROM {_keyTableName}{_keyWhereClause};
GO".Trim();

        protected string FullTreeTemplate => $@"
{KeyCreationHeader} {KeyProcedureNameFullTree}{_keyIdParameterDeclaration} AS
    SELECT * FROM {_keyFullTreeViewName}{_keyWhereClauseFullTree};
GO".Trim();

        protected override string Template =>
            DisableFullTree ? PlainObjectTemplate : PlainObjectTemplate + "\n" + FullTreeTemplate;
    }
}