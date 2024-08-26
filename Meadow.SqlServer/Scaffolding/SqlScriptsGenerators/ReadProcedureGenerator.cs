using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets.Contracts;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class ReadProcedureGeneratorPlainOnly : ReadProcedureGenerator
    {
        public ReadProcedureGeneratorPlainOnly(Type type, MeadowConfiguration configuration, bool actById)
            : base(new SnippetConstruction
            {
                EntityType = type,
                MeadowConfiguration = configuration
            }, SnippetConfigurations.IdAware(!actById))
        {
        }

        protected override bool DisableFullTree => true;
    }

    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadProcedureGeneratorFullTree : ReadProcedureGenerator
    {
        protected override bool DisableFullTree => false;

        public ReadProcedureGeneratorFullTree(SnippetConstruction construction, SnippetConfigurations configurations)
            : base(construction, configurations)
        {
        }
    }


    public abstract class ReadProcedureGenerator : SqlServerRepetitionHandlerProcedureGeneratorBase, IIdAware
    {
        public bool ActById { get; set; }

        protected abstract bool DisableFullTree { get; }

        protected ReadProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations)
            : base(construction, configurations)
        {
        }

        protected override string GetProcedureName(bool fullTree)
        {
            return ProvideDbObjectNameSupportingOverriding(() =>
            {
                if (fullTree)
                {
                    return ActById
                        ? ProcessedType.NameConvention.ReadByIdProcedureNameFullTree
                        : ProcessedType.NameConvention.ReadAllProcedureNameFullTree;
                }

                return ActById
                    ? ProcessedType.NameConvention.ReadByIdProcedureName
                    : ProcessedType.NameConvention.ReadAllProcedureName;
            });
        }

        private readonly string _keyIdParameterDeclaration = GenerateKey();

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeViewName = GenerateKey();

        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyWhereClauseFullTree = GenerateKey();
        private readonly string _keyEntityFilterSegment = GenerateKey();


        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var parameterDeclaration =
                ActById ? $"(@{ProcessedType.IdParameter.Name} {ProcessedType.IdParameter.Type})" : "";

            replacementList.Add(_keyIdParameterDeclaration, parameterDeclaration);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);
            replacementList.Add(_keyFullTreeViewName, ProcessedType.NameConvention.FullTreeViewName);

            var whereClause = ActById
                ? $" WHERE {ProcessedType.NameConvention.TableName}.{ProcessedType.IdParameter.Name}" +
                  $" = @{ProcessedType.IdParameter.Name}"
                : "";
            var whereClauseFullTree = ActById
                ? $" WHERE {ProcessedType.NameConvention.FullTreeViewName}.{ProcessedType.IdParameterFullTree.Name}" +
                  $" = @{ProcessedType.IdParameter.Name}"
                : "";

            replacementList.Add(_keyWhereClause, whereClause);
            replacementList.Add(_keyWhereClauseFullTree, whereClauseFullTree);
            
            
            var whereForEntityFilter = ActById ? " AND " : " WHERE ";
            
            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);
            
            var entityFilterSegment = entityFilterExpression.Success ? $"{whereForEntityFilter}({entityFilterExpression.Value}) " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);
        }

        protected string PlainObjectTemplate => $@"
{KeyCreationHeader} {KeyProcedureName}{_keyIdParameterDeclaration} AS
    SELECT * FROM {_keyTableName}{_keyWhereClause}{_keyEntityFilterSegment};
GO".Trim();

        protected string FullTreeTemplate => $@"
{KeyCreationHeader} {KeyProcedureNameFullTree}{_keyIdParameterDeclaration} AS
    SELECT * FROM {_keyFullTreeViewName}{_keyWhereClauseFullTree};
GO".Trim();

        protected override string Template =>
            DisableFullTree ? PlainObjectTemplate : PlainObjectTemplate + "\n" + FullTreeTemplate;
    }
}