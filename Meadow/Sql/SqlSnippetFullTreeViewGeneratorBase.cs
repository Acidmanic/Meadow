using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
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
        private readonly string _keyWhereClause = GenerateKey();

        public SqlSnippetFullTreeViewGeneratorBase
        (SnippetConstruction construction,
            SnippetConfigurations configurations,
            SnippetExecution execution)
            : base(construction, configurations, execution)
        {
            FullTreeMap = construction.MeadowConfiguration.GetFullTreeMap(EntityTypeOrOverridenEntityType);
        }

        protected Func<string, string> QuoteIfNeeded()
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
            replacementList.Add(_keyCreationHeader, GetCreationHeader());

            var whereClause = "";

            var filters = Filters();

            if (filters.Count > 0)
            {
                whereClause = " WHERE " + string.Join(" AND ", filters);
            }

            replacementList.Add(_keyWhereClause, whereClause);
        }

        protected abstract string GetCreationHeader();


        protected IEnumerable<AccessNode> GetJoinNodes()
        {
            var ev = FullTreeMap.Evaluator;
            var rootNode = ev.RootNode;

            var joinNodes = FullTreeMap.AddressKeyNodeMap.Nodes
                .Where(n => !n.IsLeaf && !n.IsCollection && n != rootNode);

            return joinNodes;
        }

        protected List<string> Filters()
        {
            var checkingNodes = GetJoinNodes().ToList();

            var rootNode = FullTreeMap.Evaluator.RootNode;

            if (checkingNodes.All(cp => cp != rootNode))
            {
                checkingNodes.Add(rootNode);
            }

            var filters = new List<string>();

            foreach (var joinNode in checkingNodes)
            {
                var nodeType = joinNode.IsAlteredType ? joinNode.AlteredType : joinNode.Type;

                var query = Construction.MeadowConfiguration.Filters
                    .Where(f => f.Key == nodeType)
                    .Select(f => f.Value)
                    .Select(query => Rebase(joinNode, query))
                    .FirstOrDefault();

                if (query is { } q)
                {
                    var translatedQuery = SqlExpressionTranslator.TranslateFilterQueryToDbExpression(q, true);

                    filters.Add(translatedQuery);
                }
            }

            return filters;
        }

        private FilterQuery Rebase(AccessNode itemNode, FilterQuery filterQuery)
        {
            var rootNode = itemNode.GetTopLevelNode();

            var rootType = rootNode.IsAlteredType ? rootNode.AlteredType : rootNode.Type;

            var items = filterQuery.Items().Select(item => Rebase(itemNode, item)).ToList();

            var rebasedQuery = new FilterQuery();

            rebasedQuery.EntityType = rootType;

            items.ForEach(rebasedQuery.Add);

            return rebasedQuery;
        }

        private FilterItem Rebase(AccessNode itemNode, FilterItem item)
        {
            var baseKey = FieldKey.Parse(itemNode.GetFullName()).Headless().ToString();

            if (string.IsNullOrWhiteSpace(baseKey))
            {
                baseKey = string.Empty;
            }
            else
            {
                baseKey += ".";
            }

            var rebasedKey = baseKey + item.Key;

            return new FilterItem
            {
                Key = rebasedKey,
                Maximum = item.Maximum,
                Minimum = item.Minimum,
                EqualityValues = item.EqualityValues,
                ValueComparison = item.ValueComparison,
                ValueType = item.ValueType
            };
        }

        private string GetInnerJoins(Func<string, string> q)
        {
            var joinNodes = GetJoinNodes();

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
    FROM   {_keyTableName}{_keyInnerJoins}{_keyWhereClause};
{_keyTaleTemplateText}
";
    }
}