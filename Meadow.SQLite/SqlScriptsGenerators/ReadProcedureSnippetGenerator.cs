using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets.Contracts;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadProcedureSnippetGeneratorFullTree : ReadProcedureSnippetGenerator
    {
        public ReadProcedureSnippetGeneratorFullTree(SnippetConstruction construction, SnippetConfigurations configurations) : base(construction, configurations)
        {
        }

        protected override bool DisableFullTree => false;
    }

    public class ReadProcedureSnippetGeneratorPlainOnly : ReadProcedureSnippetGenerator
    {
        public ReadProcedureSnippetGeneratorPlainOnly(Type type, MeadowConfiguration configuration, bool actById) :
            base(new SnippetConstruction
                {
                    EntityType = type,
                    MeadowConfiguration = configuration
                },SnippetConfigurations.IdAware(!actById))
        {
        }

        protected override bool DisableFullTree => true;
    }

    public abstract class ReadProcedureSnippetGenerator : SqLiteRepetitionHandlerProcedureGeneratorBase, IIdAware
    {
        public bool ActById { get; set; }

        protected abstract bool DisableFullTree { get; }


        protected ReadProcedureSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations)
            : base(construction, configurations)
        {
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyProcedureNameFullTree = GenerateKey();


        private readonly string _keyParametersDeclaration = GenerateKey();

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeView = GenerateKey();

        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyWhereClauseFullTree = GenerateKey();
        
        private readonly string _keyEntityFilterSegment = GenerateKey();

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            if(ActById && !ProcessedType.HasId) return;
            
            replacementList.Add(_keyProcedureName, GetProcedureName());
            replacementList.Add(_keyProcedureNameFullTree, GetProcedureNameFullTree());


            replacementList.Add(_keyParametersDeclaration,
                ActById ? $"({ParameterNameTypeJoint(ProcessedType.IdParameter, "@")})" : "");

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);
            replacementList.Add(_keyFullTreeView, ProcessedType.NameConvention.FullTreeViewName);

            var whereClause =
                ActById ? $" WHERE {ProcessedType.IdParameter?.Name} = @{ProcessedType.IdParameter?.Name}" : "";
            var whereClauseFullTree =
                ActById ? $" WHERE {ProcessedType.IdParameterFullTree?.Name} = @{ProcessedType.IdParameter?.Name}" : "";

            replacementList.Add(_keyWhereClause, whereClause);
            replacementList.Add(_keyWhereClauseFullTree, whereClauseFullTree);
            
            var whereForEntityFilter = ActById ? " AND " : " WHERE ";
            
            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);
            
            var entityFilterSegment = entityFilterExpression.Success ? $"{whereForEntityFilter}{entityFilterExpression.Value} " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);

        }


        private string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ActById
                ? ProcessedType.NameConvention.ReadByIdProcedureName
                : ProcessedType.NameConvention.ReadAllProcedureName);
        }

        private string GetProcedureNameFullTree()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ActById
                ? ProcessedType.NameConvention.ReadByIdProcedureNameFullTree
                : ProcessedType.NameConvention.ReadAllProcedureNameFullTree);
        }


        private string PlainObjectTemplate => $@"
{KeyHeaderCreation} {_keyProcedureName}{_keyParametersDeclaration} AS
    SELECT * FROM {_keyTableName}{_keyWhereClause}{_keyEntityFilterSegment};
GO
".Trim();

        private string FullTreeTemplate => $@"
{KeyHeaderCreation} {_keyProcedureNameFullTree}{_keyParametersDeclaration} AS
    SELECT * FROM {_keyFullTreeView}{_keyWhereClauseFullTree};
GO
".Trim();


        protected override string Template =>
            (ActById && !ProcessedType.HasId)?"":
                (DisableFullTree ? PlainObjectTemplate : PlainObjectTemplate + "\n" + FullTreeTemplate);
        
    }
}