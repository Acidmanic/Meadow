using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.DataTypeMapping;
using Meadow.Reflection;
using Meadow.Reflection.Conventions;
using Meadow.Reflection.ObjectTree;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Test.Functional.TestDoubles;

namespace Meadow.Test.Functional
{
    public class Tdd011CrudScripts : MeadowFunctionalTest
    {
        public override void Main()
        {
            var typesInvolved = GetTypesInvolved(typeof(Person));

            var tables = "";
            var inserts = "";
            var reads = "";
            var deletes = "";
            var updates = "";

            typesInvolved.ForEach(y =>
            {
                tables += GetTable(y);
                inserts += GetSpInsert(y);
                reads += GetSpRead(y, true, true);
                reads += GetSpRead(y, false, true);
                reads += GetSpRead(y, true, false);
                reads += GetSpRead(y, false, false);
                deletes += GetSpDelete(y, true);
                deletes += GetSpDelete(y, false);
                updates += GetSpUpdate(y);
            });

            Console.WriteLine(tables + inserts + reads + deletes + updates);
        }

        private string GetTable(Type type)
        {
            var node = new TypeAnalyzer().ToAccessNode(type, false);

            var info = new AccessTreeInformation(node);

            var tableName = GetTableName(type);

            var tableScript = $"CREATE TABLE {tableName} (\n\t";

            var sep = "";

            var typeMap = new SqlDbTypeNameMapper();

            foreach (var leaf in info.OrderedLeaves)
            {
                string fieldName = leaf.Name;

                string typeName = typeMap[leaf.Type];

                string id = leaf.IsUnique ? " IDENTITY (1,1) NOT NULL PRIMARY KEY" : "";

                tableScript += sep + fieldName + " " + typeName + id;

                sep = ", ";
            }

            tableScript += "\n\t)\n\n";

            return tableScript;
        }


        private string GetSpInsert(Type type)
        {
            var node = new TypeAnalyzer().ToAccessNode(type, false);

            var info = new AccessTreeInformation(node);

            var tableName = GetTableName(type);


            var typeMap = new SqlDbTypeNameMapper();

            var sep = "";
            var fields = "";
            var parameters = "";
            var values = "";
            var idFieldName = "";
            var idFieldType = "";
            foreach (var leaf in info.OrderedLeaves)
            {
                if (leaf.IsUnique)
                {
                    idFieldName = leaf.Name;
                    idFieldType = typeMap[leaf.Type];
                }
                else
                {
                    string fieldName = leaf.Name;

                    string typeName = typeMap[leaf.Type];

                    fields += sep + fieldName;

                    parameters += sep + "@" + fieldName + " " + typeName;

                    values += sep + "@" + fieldName;

                    sep = ", ";
                }
            }

            var script = $"CREATE PROCEDURE spInsert{type.Name} (\n\t{parameters}\n)AS\n";

            script += $"\tINSERT INTO {tableName} ({fields}) VALUES ({values})\n";

            script += $"\tDECLARE @newId {idFieldType}=(IDENT_CURRENT('{tableName}'));\n";

            script += $"\tSELECT * FROM {tableName} WHERE {idFieldName}=@newId;\n";

            script += "GO\n\n";

            return script;
        }


        private class FullTreeSelectState
        {
            public string Joins { get; set; }

            public string Parameters { get; set; }

            public string TableName { get; set; }

            public string Sep { get; set; }

            public AccessTreeInformation Info { get; set; }

            public override string ToString()
            {
                return $"SELECT {Parameters} FROM {TableName} {Joins}";
            }
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

                result.Joins += result.Sep + "LEFT " + GetJoin(father, node,false);

                result.Sep = " ";
            }
            else if (!node.IsLeaf && !node.IsCollection && !node.IsRoot)
            {
                var father = node.Parent;

                result.Joins += result.Sep + GetJoin(father, node,true);

                result.Sep = " ";
            }

            node.GetChildren().ForEach(child => ExtractSelectInfo(child, result));
        }


        private string GetJoin(AccessNode father, AccessNode joining,bool direction1ton)
        {
            var jTableName = GetTableName(joining.Type);

            var fTableName = GetTableName(father.Type);

            string idCheck = "";
            
            if (direction1ton)
            {
                var id = joining.GetChildren()
                    .SingleOrDefault(child => child.IsLeaf && child.IsUnique);
                
                var nodeId = joining.Type.Name + id.Name;
                
                idCheck = $"{fTableName}.{nodeId}={jTableName}.{id.Name}";
            }
            else
            {
                var id = father.GetChildren()
                    .SingleOrDefault(child => child.IsLeaf && child.IsUnique);

                var nodeId = father.Type.Name + id.Name;
                
                idCheck = $"{jTableName}.{nodeId}={fTableName}.{id.Name}";
            }
            
            return $"JOIN {jTableName} on {idCheck}";
        }

        public string GetSpRead(Type type, bool byId, bool fullTree)
        {
            var tableName = GetTableName(type);

            var entityName = type.Name;

            var idField = GetIdField(type);

            var typeMap = new SqlDbTypeNameMapper();

            var spName = "spGet" + (byId ? $"{entityName}ById" : $"All{tableName}") + (fullTree ? "FullTree" : "");

            var parameters = byId ? $"@{idField.Name} {typeMap[idField.Type]}" : "";

            var script = $"CREATE PROCEDURE {spName}({parameters})\nAS";

            var where = byId ? $"WHERE {idField.Name}=@{idField.Name}" : "";

            var select = $"SELECT * FROM {tableName}";

            if (fullTree)
            {
                var node = new TypeAnalyzer().ToAccessNode(type, true);

                var sel = ExtractSelectInfo(node);

                select = sel.ToString();
            }

            script += $"\n\t{select} {where}\nGO\n\n";

            return script;
        }

        public string GetSpDelete(Type type, bool byId)
        {
            var tableName = GetTableName(type);

            var entityName = type.Name;

            var idField = GetIdField(type);

            var typeMap = new SqlDbTypeNameMapper();

            var spName = "spDelete" + (byId ? $"{entityName}ById" : $"All{tableName}");

            var parameters = byId ? $"@{idField.Name} {typeMap[idField.Type]}" : "";

            var script = $"CREATE PROCEDURE {spName}({parameters})\nAS";

            var where = byId ? $" WHERE {idField.Name}=@{idField.Name}" : "";

            script += $"\n\tDECLARE @existing = (SELECT COUNT(*) FROM {tableName});";

            script += $"\n\tDELETE FROM {tableName}{where}";

            script += $"\n\tDECLARE @delta = @existing - (SELECT COUNT(*) FROM {tableName});";

            script += "\n\tIF @delta > 0 or @existing = 0\n\t\tSELECT cast(1 as bit) Success";

            script += "\n\tELSE\n\t\tselect cast(0 as bit) Success\nGO\n\n";

            return script;
        }

        private string GetSpUpdate(Type type)
        {
            var node = new TypeAnalyzer().ToAccessNode(type, false);

            var info = new AccessTreeInformation(node);

            var tableName = GetTableName(type);


            var typeMap = new SqlDbTypeNameMapper();

            var sep = "";
            var parameters = "";
            var idFieldName = "";
            var idFieldType = "";
            var columnValues = "";
            foreach (var leaf in info.OrderedLeaves)
            {
                if (leaf.IsUnique)
                {
                    idFieldName = leaf.Name;
                    idFieldType = typeMap[leaf.Type];
                }
                else
                {
                    string fieldName = leaf.Name;

                    string typeName = typeMap[leaf.Type];

                    parameters += sep + "@" + fieldName + " " + typeName;

                    columnValues += sep + fieldName + " = @" + fieldName;

                    sep = ", ";
                }
            }

            var script = $"CREATE PROCEDURE spUpdate{type.Name} (\n\t@{idFieldName} {idFieldType} ,{parameters})\nAS";

            script += $"\n\tUPDATE {tableName}";

            script += $"\n\tSET {columnValues}";

            script += $"\n\tWHERE {idFieldName}=@{idFieldName}";

            script += $"\n\tSELECT * FROM {tableName} WHERE {idFieldName}=@{idFieldName};";

            script += "\nGO\n\n";

            return script;
        }

        public AccessNode GetIdField(Type type)
        {
            var leaves = new TypeAnalyzer().ToAccessNode(type, false).EnumerateLeavesBelow();

            return leaves.SingleOrDefault(l => l.IsUnique);
        }

        private string GetTableName(Type type)
        {
            return new PluralTableNameProvider().GetTableName(type);
        }

        public List<Type> GetTypesInvolved(Type type)
        {
            var result = new List<Type>();

            EnumerateTypesInvolved(type, result);

            return result;
        }

        private void EnumerateTypesInvolved(Type type, List<Type> result)
        {
            if (TypeCheck.IsReferenceType(type))
            {
                if (TypeCheck.IsCollection(type))
                {
                    type = type.GenericTypeArguments[0];
                }

                result.Add(type);
            }

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;

                EnumerateTypesInvolved(pType, result);
            }
        }
    }
}