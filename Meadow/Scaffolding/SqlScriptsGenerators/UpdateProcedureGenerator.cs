using System;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    public class UpdateProcedureGenerator:ProcedureGenerator
    {
        public UpdateProcedureGenerator(Type type) : base(type)
        {
        }

        protected override string GetProcedureName()
        {
            return $"spUpdate{EntityName}";
        }

        protected override string GenerateScript(bool alreadyExists, string createKeyword)
        {
            
            var sep = "";
            var parameters = "";
            var idFieldName = "";
            var idFieldType = "";
            var columnValues = "";
            
            WalkThroughLeaves(false, leaf =>
            {
                if (leaf.IsUnique)
                {
                    idFieldName = leaf.Name;
                    idFieldType = TypeNameMapper[leaf.Type];
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
            
            var script = $"{createKeyword} PROCEDURE {ProcedureName} (\n\t@{idFieldName} {idFieldType} ,{parameters})\nAS";

            script += $"\n\tUPDATE {TableName}";

            script += $"\n\tSET {columnValues}";

            script += $"\n\tWHERE {idFieldName}=@{idFieldName}";

            script += $"\n\tSELECT * FROM {TableName} WHERE {idFieldName}=@{idFieldName};";

            script += "\nGO\n\n";

            return script;
        }
    }
}