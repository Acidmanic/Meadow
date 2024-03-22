using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Attributes;
using Meadow.DataTypeMapping;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Sql
{
    public abstract class SqlSnippetFullTreeViewGeneratorBase : ByTemplateSqlSnippetGeneratorBase
    {
        protected FullTreeMap FullTreeMap { get; }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParametersTable = GenerateKey();
        private readonly string _keyInnerJoins = GenerateKey();
        private readonly string _keyLeadingTemplateText = GenerateKey();
        private readonly string _keyTaleTemplateText = GenerateKey();
        private readonly string _keyViewName = GenerateKey();
        private readonly string _keyCreationHeader = GenerateKey();

        public SqlSnippetFullTreeViewGeneratorBase
            (IDbTypeNameMapper mapper,
                SnippetConstruction construction,
                SnippetConfigurations configurations)
            : base(mapper, construction, configurations)
        {

            FullTreeMap = construction.MeadowConfiguration.GetFullTreeMap(EntityTypeOrOverridenEntityType);
        }

        protected Func<string,string> QuoteIfNeeded()
        {
            if (DoubleQuoteIdentifiers)
            {
                return s => $"\"{s}\"";
            }

            return s => s;
        }
        
        protected string GetViewName()
        {
            return ProvideDbObjectNameSupportingOverriding(GetOriginalViewName);
        }
        
        private string GetOriginalViewName()
        {
            var tableName = ProcessedType.NameConvention.TableName;

            var viewName = tableName + "FullTree";

            return viewName;
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            Func<string, string> q = QuoteIfNeeded();

            var tableName = ProcessedType.NameConvention.TableName;

            var viewName = GetViewName();

            replacementList.Add(_keyTableName, q(tableName));
            
            replacementList.Add(_keyViewName, q(viewName));

            var parametersTable = GetParametersTable(q);

            replacementList.Add(_keyParametersTable, parametersTable);

            var innerJoins = GetInnerJoins(q);

            replacementList.Add(_keyInnerJoins, innerJoins);

            replacementList.Add(_keyLeadingTemplateText, LeadingTemplateText());
            replacementList.Add(_keyTaleTemplateText, TaleTemplateText());
            replacementList.Add(_keyCreationHeader,GetCreationHeader());
        }

        protected abstract string GetCreationHeader();
        
        private string GetInnerJoins(Func<string, string> q)
        {
            var ev = FullTreeMap.Evaluator;
            var rootNode = ev.RootNode;

            var joinNodes = FullTreeMap.AddressKeyNodeMap.Nodes
                .Where(n => !n.IsLeaf && !n.IsCollection && n != rootNode);

            var joins = "";
            foreach (var joinNode in joinNodes)
            {
                AccessNode pointerNode, nodePointedAt;
                if (joinNode.IsCollectable)
                {
                    // N -> 1
                    pointerNode = joinNode;
                    // The grandFather
                    nodePointedAt = joinNode.Parent.Parent;
                }
                else
                {
                    //The parent 
                    pointerNode = joinNode.Parent;
                    nodePointedAt = joinNode;
                }

                string joinTerm = GetJoinTerm(joinNode, pointerNode, nodePointedAt, q);
                joins += "\n        " + joinTerm;
            }

            return joins;
        }


        private string GetReferencedIdFieldName(AccessNode node)
        {
            var foundAttribute = node.Type.GetCustomAttributes(true).FirstOrDefault(a => a is OneToMany);

            if (foundAttribute is OneToMany nToOneAtt)
            {
                return nToOneAtt.ReferenceFieldName;
            }

            return node.Name + "Id";
        }
        
        
        private string GetJoinTerm(AccessNode joinNode, AccessNode pointerNode, AccessNode nodePointedAt,
            Func<string, string> q)
        {
            var joinTableName = ProcessedType.NameConvention.TableNameProvider.GetNameForOwnerType(joinNode.Type);
            var pointerTableName = ProcessedType.NameConvention.TableNameProvider.GetNameForOwnerType(pointerNode.Type);
            var pointerIdFieldName = GetReferencedIdFieldName(nodePointedAt);
            var pointedAtTableName =
                ProcessedType.NameConvention.TableNameProvider.GetNameForOwnerType(nodePointedAt.Type);
            var pointedAtIdField = TypeIdentity.FindIdentityLeaf(nodePointedAt.Type).Name;

            return
                $"LEFT JOIN {q(joinTableName)} ON {q(pointerTableName)}.{q(pointerIdFieldName)} =" +
                $" {q(pointedAtTableName)}.{q(pointedAtIdField)}";
        }


        private string GetParametersTable(Func<string, string> q)
        {
            var relationalMap = FullTreeMap.RelationalMap;
            var parameterTable = "";
            var tab = "        ";
            var sep = "";
            foreach (var columnKey in relationalMap)
            {
                var fullTreeAlias = columnKey.Key;
                var key = columnKey.Value;
                var node = FullTreeMap.AddressKeyNodeMap.NodeByKey(key);
                var ownerType = node.Parent.Type;
                var tableName = ProcessedType.NameConvention.TableNameProvider.GetNameForOwnerType(ownerType);
                var originalColumnName = q(tableName) + "." + q(node.Name);

                parameterTable += sep + tab + originalColumnName + tab + $"{AliasQuote}{fullTreeAlias}{AliasQuote}";
                sep = ",\n";
            }

            return parameterTable.Trim();
        }

        protected static readonly string Split = @"
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------"
            .Trim();
        
        protected static readonly string Line = @"
-- ---------------------------------------------------------------------------------------------------------------------"
            .Trim();

        protected virtual string LeadingTemplateText()
        {
            return Line;
        }

        protected virtual string TaleTemplateText()
        {
            return Line;
        }

        protected virtual bool DoubleQuoteIdentifiers => false;

        protected virtual string AliasQuote => "'";

        protected override string Template => $@"
{_keyLeadingTemplateText}
{_keyCreationHeader} {_keyViewName} AS 
    SELECT {_keyParametersTable}
    FROM   {_keyTableName}{_keyInnerJoins};
{_keyTaleTemplateText}
";
    }
}