using System;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class ReadProcedureGenerator : SQLite.SqlScriptsGenerators.ProcedureGenerator
    {
        public bool ById { get; }
        public bool FullTree { get; }

        public int Top { get; }

        public bool OrderAscending { get; }

        public ReadProcedureGenerator(Type type, bool byId, bool fullTree) : base(type)
        {
            ById = byId;
            FullTree = fullTree;
            Top = 0;
            OrderAscending = false;
        }

        public ReadProcedureGenerator(Type type, bool byId, bool fullTree, int top, bool orderAscending) : base(type)
        {
            ById = byId;
            FullTree = fullTree;
            Top = top;
            OrderAscending = orderAscending;
        }

        protected override string GenerateScript(SqlScriptActions action, string snippet)
        {

            var useIdField = ById && HasIdField;

            var parameters = useIdField ? $"(@{IdField.Name} {TypeNameMapper[IdField.Type]})" : "";

            var script = $"{snippet} PROCEDURE {ProcedureName}{parameters}\nAS";

            var where = useIdField ? $"WHERE {IdField.Name}=@{IdField.Name}" : "";

            var top = GetTop();

            var order = GetOrder(useIdField, IdField?.Name);

            var select = $"SELECT {top} * FROM {NameConvention.TableName} {order}";

            if (FullTree)
            {
                var sel = ExtractSelectInfo(TreeRoot);

                select = sel.ToString();
            }

            script += $"\n\t{select} {where}\nGO\n\n";

            return script;
        }

        private string GetOrder(bool useIdField, string idFieldName)
        {
            if (useIdField)
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
                return $"TOP {Top}";
            }

            return "";
        }

        protected override string GetProcedureName()
        {
            return ById
                ? (FullTree ? NameConvention.SelectByIdProcedureNameFullTree : NameConvention.SelectByIdProcedureName)
                : (FullTree ? NameConvention.SelectAllProcedureNameFullTree : NameConvention.SelectAllProcedureName);
        }

        private FullTreeSelectState ExtractSelectInfo(AccessNode node)
        {
            var type = node.Type;

            var tableName = NameConvention.TableNameProvider.GetNameForOwnerType(type);

            var result = new FullTreeSelectState
            {
                TableName = tableName,
                Parameters = "",
                Joins = "",
                Sep = "",
                Info = new AccessTreeInformation(node)
            };

            ExtractSelectInfo(node, result);

            var sep = "";
            foreach (var filed in result.Info.OrderedFieldNames)
            {
                result.Parameters += sep + filed + $" '{filed}'";

                sep = ", ";
            }

            return result;
        }

        private void ExtractSelectInfo(AccessNode node, FullTreeSelectState result)
        {
            if (node.IsCollectable)
            {
                var father = node.Parent.Parent;

                result.Joins += result.Sep + "LEFT " + GetJoin(father, node, false,result.Info);

                result.Sep = " ";
            }
            else if (!node.IsLeaf && !node.IsCollection && !node.IsRoot)
            {
                var father = node.Parent;

                result.Joins += result.Sep + GetJoin(father, node, true,result.Info);

                result.Sep = " ";
            }

            node.GetChildren().ForEach(child => ExtractSelectInfo(child, result));
        }

        private string GetJoin(AccessNode father, AccessNode joining, bool direction1Ton,AccessTreeInformation info)
        {
            var jTableName = NameConvention.TableNameProvider.GetNameForOwnerType(joining.Type);

            var fTableName = NameConvention.TableNameProvider.GetNameForOwnerType(father.Type);

            string idCheck = "";

            var from = direction1Ton ? joining : father;
            var to = direction1Ton ? father : joining;
            var fromTableName = direction1Ton ? jTableName : fTableName;
            var toTableName = direction1Ton ? fTableName : jTableName;


            var idNode = from.GetChildren()
                .SingleOrDefault(child => child.IsLeaf && child.IsUnique);

            if (idNode == null)
            {
                // A Node without unique id cant participate in a join
                return "";
            }

            var nodeId =  NameConvention.TableNameProvider.GetNameForOwnerType(from.Type) + idNode.Name;

            idCheck = $"{toTableName}.{nodeId}={fromTableName}.{idNode.Name}";

            return $"JOIN {jTableName} on {idCheck}";
        }
    }
}