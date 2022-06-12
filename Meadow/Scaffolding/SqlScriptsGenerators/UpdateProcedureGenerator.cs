using System;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    public class UpdateProcedureGenerator : ProcedureGenerator
    {
        public UpdateProcedureGenerator(Type type) : base(type)
        {
        }

        protected override string GetProcedureName()
        {
            return NameConvention.UpdateProcedureName;
        }

        protected override string GenerateScript(SqlScriptActions action, string snippet)
        {
            var sep = "";
            var parameters = "";
            var idFieldName = "";
            var idFieldType = "";
            var columnValues = "";
            AccessNode idLeaf = null;

            WalkThroughLeaves(false, leaf =>
            {
                if (leaf.IsUnique)
                {
                    idFieldName = leaf.Name;
                    idFieldType = TypeNameMapper[leaf.Type];
                    idLeaf = leaf;
                }
                else
                {
                    string fieldName = leaf.Name;

                    string typeName = TypeNameMapper[leaf.Type];

                    parameters += sep + "@" + fieldName + " " + typeName;

                    columnValues += sep + fieldName + " = @" + fieldName;

                    sep = ", ";
                }
            });

            if (idLeaf == null)
            {
                // An entity without Id, cant be updated by Id!!
                return "";
            }

            var script = $"{snippet} PROCEDURE {ProcedureName} (\n\t@{idFieldName} {idFieldType} ,{parameters})\nAS";

            script += $"\n\tUPDATE {NameConvention.TableName}";

            script += $"\n\tSET {columnValues}";

            script += $"\n\tWHERE {idFieldName}=@{idFieldName}";

            script += $"\n\tSELECT * FROM {NameConvention.TableName} WHERE {idFieldName}=@{idFieldName};";

            script += "\nGO\n\n";

            return script;
        }
    }
}