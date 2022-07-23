using System;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.SqlScriptsGenerators;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class InsertProcedureGenerator : ProcedureGenerator
    {
       

        public InsertProcedureGenerator(Type type) : base(type)
        {
            UseDbTypeMapper(new MySqlDbTypeNameMapper());
        }

        protected override string GenerateScript(SqlScriptActions action, string snippet)
        {
            var sep = "";
            var fields = "";
            var parameters = "";
            var values = "";
            var idFieldName = "";
            var idFieldType = "";
            var hadId = false;

            WalkThroughLeaves(false, leaf =>
            {
                if (leaf.IsUnique)
                {
                    idFieldName = leaf.Name;
                    idFieldType = TypeNameMapper[leaf.Type];
                    hadId = true;
                }
                else
                {
                    string fieldName = leaf.Name;

                    string typeName = TypeNameMapper[leaf.Type];

                    fields += sep + fieldName;

                    parameters += sep + "IN " + fieldName + " " + typeName;

                    values += sep + fieldName;

                    sep = ", ";
                }
            });

            var script = $"{snippet} PROCEDURE {ProcedureName} (\n\t{parameters}\n)\nBEGIN\n";

            script += $"\tINSERT INTO {NameConvention.TableName} ({fields})";
            
            script += $" VALUES ({values});\n";

            if (hadId)
            {
                script += $"\tSELECT * FROM {NameConvention.TableName} WHERE {idFieldName}=last_insert_id();\n";
            }

            script += "END\n\n";

            return script;
        }

        protected override string GetProcedureName()
        {
            return NameConvention.InsertProcedureName;
        }
    }
}