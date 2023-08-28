using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class FullTreeViewGenerator : ByTemplateSqlGeneratorBase
    {
        protected Type EntityType { get;  }
        protected ProcessedType ProcessedType { get;  }
        
        protected FullTreeMap FullTreeMap { get; }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyParametersTable = GenerateKey();
        private readonly string _keyInnerJoins = GenerateKey();

        public FullTreeViewGenerator(Type type) : base(new SqLiteTypeNameMapper())
        {
            EntityType = type;
            ProcessedType = Process(type);
            FullTreeMap = new FullTreeMap(EntityType, '_',
                ProcessedType.NameConvention.TableNameProvider);
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var parametersTable = GetParametersTable();

            replacementList.Add(_keyParametersTable, parametersTable);

            var innerJoins = GetInnerJoins();
            
            replacementList.Add(_keyInnerJoins, innerJoins);
        }

        private string GetInnerJoins()
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

                string joinTerm = GetJoinTerm(joinNode, pointerNode, nodePointedAt);
                joins += "\n        " + joinTerm;
            }

            return joins;
        }

        private string GetJoinTerm(AccessNode joinNode, AccessNode pointerNode, AccessNode nodePointedAt)
        {
            var joinTableName = ProcessedType.NameConvention.TableNameProvider.GetNameForOwnerType(joinNode.Type);
            var pointerTableName = ProcessedType.NameConvention.TableNameProvider.GetNameForOwnerType(pointerNode.Type);
            var pointerIdFieldName = nodePointedAt.Name + "Id";
            var pointedAtTableName = ProcessedType.NameConvention.TableNameProvider.GetNameForOwnerType(nodePointedAt.Type);
            var pointedAtIdField = TypeIdentity.FindIdentityLeaf(nodePointedAt.Type).Name;

            return $"INNER JOIN {joinTableName} ON {pointerTableName}.{pointerIdFieldName} = {pointedAtTableName}.{pointedAtIdField}";
        }


        private string GetParametersTable()
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
                var originalColumnName = tableName + "." + node.Name;

                parameterTable += sep + tab + originalColumnName + tab + $"'{fullTreeAlias}'";
                sep = ",\n";
            }

            return parameterTable.Trim();
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE VIEW {_keyTableName}FullTree AS 
    SELECT {_keyParametersTable}
    FROM   {_keyTableName}{_keyInnerJoins};
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
";
    }
}