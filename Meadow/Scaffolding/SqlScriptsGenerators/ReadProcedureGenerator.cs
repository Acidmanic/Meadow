using System;
using System.Linq;
using Meadow.Reflection;
using Meadow.Reflection.ObjectTree;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    public class ReadProcedureGenerator : ProcedureGenerator
    {
        public bool ById { get; }
        public bool FullTree { get; }

        public ReadProcedureGenerator(Type type, bool byId, bool fullTree) : base(type)
        {
            ById = byId;
            FullTree = fullTree;
        }

        protected override string GenerateScript(SqlScriptActions action, string snippet)
        {
            var idField = GetIdField(Type);

            var useIdField = ById && idField != null;

            var parameters = useIdField ? $"@{idField.Name} {TypeNameMapper[idField.Type]}" : "";

            var script = $"{snippet} PROCEDURE {ProcedureName}({parameters})\nAS";

            var where = useIdField ? $"WHERE {idField.Name}=@{idField.Name}" : "";

            var select = $"SELECT * FROM {TableName}";

            if (FullTree)
            {
                var sel = ExtractSelectInfo(TreeRoot);

                select = sel.ToString();
            }

            script += $"\n\t{select} {where}\nGO\n\n";

            return script;
        }

        protected override string GetProcedureName()
        {
            var spName = "spGet" + (ById ? $"{EntityName}ById" : $"All{TableName}") + (FullTree ? "FullTree" : "");

            return spName;
        }

        private FullTreeSelectState ExtractSelectInfo(AccessNode node)
        {
            var type = node.Type;

            var tableName = GetTableName(type);

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
                result.Parameters += sep + filed;

                sep = ", ";
            }

            return result;
        }

        private void ExtractSelectInfo(AccessNode node, FullTreeSelectState result)
        {
            if (node.IsCollectable)
            {
                var father = node.Parent.Parent;

                result.Joins += result.Sep + "LEFT " + GetJoin(father, node, false);

                result.Sep = " ";
            }
            else if (!node.IsLeaf && !node.IsCollection && !node.IsRoot)
            {
                var father = node.Parent;

                result.Joins += result.Sep + GetJoin(father, node, true);

                result.Sep = " ";
            }

            node.GetChildren().ForEach(child => ExtractSelectInfo(child, result));
        }

        private string GetJoin(AccessNode father, AccessNode joining, bool direction1Ton)
        {
            var jTableName = GetTableName(joining.Type);

            var fTableName = GetTableName(father.Type);

            string idCheck = "";

            var from = direction1Ton ? joining : father;
            var to = direction1Ton ? father : joining;
            var fromTableName = direction1Ton ? jTableName : fTableName;
            var toTableName = direction1Ton ? fTableName : jTableName;


            var id = from.GetChildren()
                .SingleOrDefault(child => child.IsLeaf && child.IsUnique);

            if (id == null)
            {
                // A Node without unique id cant participate in a join
                return "";
            }
            var nodeId = from.Type.Name + id.Name;

            idCheck = $"{toTableName}.{nodeId}={fromTableName}.{id.Name}";
            
            //
            // if (direction1Ton)
            // {
            //     var id = joining.GetChildren()
            //         .SingleOrDefault(child => child.IsLeaf && child.IsUnique);
            //
            //     var nodeId = joining.Type.Name + id.Name;
            //
            //     idCheck = $"{fTableName}.{nodeId}={jTableName}.{id.Name}";
            // }
            // else
            // {
            //     var id = father.GetChildren()
            //         .SingleOrDefault(child => child.IsLeaf && child.IsUnique);
            //
            //     var nodeId = father.Type.Name + id.Name;
            //
            //     idCheck = $"{jTableName}.{nodeId}={fTableName}.{id.Name}";
            // }

            return $"JOIN {jTableName} on {idCheck}";
        }
    }
}