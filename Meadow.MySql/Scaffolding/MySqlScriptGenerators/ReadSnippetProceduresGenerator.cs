using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets.Contracts;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class ReadSnippetProcedureGenerator<TEntity> : ReadSequenceSnippetProcedureGenerator<TEntity>
    {
        public ReadSnippetProcedureGenerator(MeadowConfiguration configuration, bool allNotById) :
            base(configuration, allNotById, 0)
        {
        }
    }

    public class ReadSequenceSnippetProcedureGenerator<TEntity> : ReadSequenceSnippetProcedureGenerator
    {
        public ReadSequenceSnippetProcedureGenerator(MeadowConfiguration configuration, bool allNotById, int top,
            bool orderById = false,
            bool orderAscending = true)
            : base(new SnippetConstruction
            {
                EntityType = typeof(TEntity),
                MeadowConfiguration = configuration
            }, SnippetConfigurations.IdAware(allNotById), top, orderById, orderAscending)
        {
        }
    }

    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadSnippetProcedureGenerator : ReadSequenceSnippetProcedureGenerator
    {
        public ReadSnippetProcedureGenerator(SnippetConstruction construction, SnippetConfigurations configurations) :
            base(construction, configurations)
        {
        }
    }

    public class ReadSequenceSnippetProcedureGenerator : MySqlRepetitionHandlerProcedureGeneratorBase, IIdAware
    {
        public int Top { get; }

        public bool OrderAscending { get; }

        public bool OrderById { get; }

        public bool ActById { get; set; }

        public ReadSequenceSnippetProcedureGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations)
            : this(construction, configurations, 0, false, true)
        {
        }

        public ReadSequenceSnippetProcedureGenerator(
            SnippetConstruction construction,
            SnippetConfigurations configurations
            , int top, bool orderById = false, bool orderAscending = true) : base(construction, configurations)
        {
            Top = top;
            OrderAscending = orderAscending;
            OrderById = orderById;
        }

        private string GetOrder(string idFieldName)
        {
            if (OrderById)
            {
                if (Top > 0)
                {
                    var ascDesc = OrderAscending ? "ASC" : "DESC";

                    return $"ORDER BY {idFieldName} {ascDesc}";
                }
            }

            return "";
        }

        private string GetTop()
        {
            if (Top > 0)
            {
                return $"LIMIT {Top}";
            }

            return "";
        }

        protected override string GetProcedureName(bool fullTree)
        {
            return ProvideDbObjectNameSupportingOverriding(() => GetOriginalProcedureName(fullTree));
        }
        
        private string GetOriginalProcedureName(bool fullTree)
        {
            var nameConvention = ProcessedType.NameConvention;

            if (Configurations.OverrideDbObjectName)
            {
                return Configurations.OverrideDbObjectName.Value(Construction);
            }

            if (!ActById)
            {
                return (Top > 0) switch
                {
                    true when OrderAscending => fullTree
                        ? nameConvention.SelectFirstProcedureNameFullTree
                        : nameConvention.SelectFirstProcedureName,
                    true => fullTree
                        ? nameConvention.SelectLastProcedureNameFullTree
                        : nameConvention.SelectLastProcedureName,
                    _ => fullTree
                        ? nameConvention.SelectAllProcedureNameFullTree
                        : nameConvention.SelectAllProcedureName
                };
            }

            return fullTree ? nameConvention.SelectByIdProcedureNameFullTree : nameConvention.SelectByIdProcedureName;
        }

        private readonly string _keyIdParam = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyTopClause = GenerateKey();
        private readonly string _keyOrderClause = GenerateKey();

        private readonly string _keyFullTreeViewName = GenerateKey();
        private readonly string _keyWhereClauseFullTree = GenerateKey();
        private readonly string _keyOrderClauseFullTree = GenerateKey();
        
        private readonly string _keyEntityFilterSegment = GenerateKey();
        private readonly string _keyEntityFilterSegmentFullTree = GenerateKey();


        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            if (ActById && !ProcessedType.HasId)
            {
                return;
            }

            replacementList.Add(_keyIdParam,
                !ActById ? "" : ("IN " + ProcessedType.IdParameter.Name + " " + ProcessedType.IdParameter.Type));

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyFullTreeViewName, ProcessedType.NameConvention.FullTreeViewName);

            replacementList.Add(_keyWhereClause, !ActById
                ? ""
                : ("WHERE " + ProcessedType.NameConvention.TableName + "."
                   + ProcessedType.IdParameter.Name + " = " + ProcessedType.IdParameter.Name));

            replacementList.Add(_keyWhereClauseFullTree, !ActById
                ? ""
                : ("WHERE " + ProcessedType.NameConvention.FullTreeViewName + "."
                   + ProcessedType.IdParameterFullTree.Name + " = " + ProcessedType.IdParameter.Name));

            var orderParameter = ProcessedType.HasId ? ProcessedType.IdParameter:ProcessedType.Parameters[0];
            
            var fullTreeOrderParameter = ProcessedType.HasId ? ProcessedType.IdParameter:ProcessedType.ParametersFullTree[0];
            
            replacementList.Add(_keyOrderClause, GetOrder(orderParameter.Name));

            replacementList.Add(_keyOrderClauseFullTree, GetOrder(fullTreeOrderParameter.Name));

            replacementList.Add(_keyTopClause, GetTop());

            var whereForEntityFilter = ActById ? " AND " : " WHERE ";
            
            var entityFilterExpression = GetFiltersWhereClause(false);
            
            var entityFilterSegment = entityFilterExpression.Success ? $"{whereForEntityFilter}({entityFilterExpression.Value}) " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);
            
            var entityFilterExpressionFullTree = GetFiltersWhereClause(true);

            var entityFilterSegmentFullTree = entityFilterExpressionFullTree.Success ? $"{whereForEntityFilter}({entityFilterExpressionFullTree.Value}) " : "";
            
            replacementList.Add(_keyEntityFilterSegmentFullTree,entityFilterSegmentFullTree);
        }

        protected override string Template =>(ActById && !ProcessedType.HasId)?"": @$"
{KeyCreationHeader} {KeyProcedureName}({_keyIdParam})
BEGIN
    SELECT * FROM {_keyTableName} {_keyWhereClause}{_keyEntityFilterSegment}{_keyOrderClause} {_keyTopClause};
END;
-- ---------------------------------------------------------------------------------------------------------------------
{KeyCreationHeaderFullTree} {KeyProcedureNameFullTree}({_keyIdParam})
BEGIN
    SELECT * FROM {_keyFullTreeViewName} {_keyWhereClauseFullTree}{_keyEntityFilterSegmentFullTree}{_keyOrderClauseFullTree} {_keyTopClause};
END;
".Trim();
    }
}