using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.Sql
{
    public class SqlFullTreeViewGeneratorBase : ByTemplateSqlGeneratorBase
    {
        protected Type EntityType { get; }
        protected ProcessedType ProcessedType { get; }

        protected FullTreeMap FullTreeMap { get; }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParametersTable = GenerateKey();
        private readonly string _keyInnerJoins = GenerateKey();
        private readonly string _keyLeadingTemplateText = GenerateKey();
        private readonly string _keyTaleTemplateText = GenerateKey();
        private readonly string _keyViewName = GenerateKey();

        public SqlFullTreeViewGeneratorBase(Type type, MeadowConfiguration configuration, IDbTypeNameMapper mapper)
            : base(mapper, configuration)
        {
            EntityType = type;

            ProcessedType = Process(EntityType);
            FullTreeMap = configuration.GetFullTreeMap(EntityType);
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            Func<string, string> q = s => s;

            if (DoubleQuoteIdentifiers)
            {
                q = s => $"\"{s}\"";
            }

            var tableName = ProcessedType.NameConvention.TableName;

            var viewName = tableName + "FullTree";

            replacementList.Add(_keyTableName, q(tableName));
            replacementList.Add(_keyViewName, q(viewName));

            var parametersTable = GetParametersTable(q);

            replacementList.Add(_keyParametersTable, parametersTable);

            var innerJoins = GetInnerJoins(q);

            replacementList.Add(_keyInnerJoins, innerJoins);

            replacementList.Add(_keyLeadingTemplateText, LeadingTemplateText());
            replacementList.Add(_keyTaleTemplateText, TaleTemplateText());
        }

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

        private string GetJoinTerm(AccessNode joinNode, AccessNode pointerNode, AccessNode nodePointedAt,
            Func<string, string> q)
        {
            var joinTableName = ProcessedType.NameConvention.TableNameProvider.GetNameForOwnerType(joinNode.Type);
            var pointerTableName = ProcessedType.NameConvention.TableNameProvider.GetNameForOwnerType(pointerNode.Type);
            var pointerIdFieldName = nodePointedAt.Name + "Id";
            var pointedAtTableName =
                ProcessedType.NameConvention.TableNameProvider.GetNameForOwnerType(nodePointedAt.Type);
            var pointedAtIdField = TypeIdentity.FindIdentityLeaf(nodePointedAt.Type).Name;

            return
                $"INNER JOIN {q(joinTableName)} ON {q(pointerTableName)}.{q(pointerIdFieldName)} =" +
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


        protected virtual string LeadingTemplateText()
        {
            return
                "-- ---------------------------------------------------------------------------------------------------------------------";
        }

        protected virtual string TaleTemplateText()
        {
            return
                "-- ---------------------------------------------------------------------------------------------------------------------";
        }

        protected virtual bool DoubleQuoteIdentifiers => false;

        protected virtual string AliasQuote => "'";

        protected override string Template => $@"
{_keyLeadingTemplateText}
CREATE VIEW {_keyViewName} AS 
    SELECT {_keyParametersTable}
    FROM   {_keyTableName}{_keyInnerJoins};
{_keyTaleTemplateText}
";
    }
}