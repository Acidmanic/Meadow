using System;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.DataTypeMapping;
using Meadow.Reflection;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    public class InsertProcedureGenerator : ProcedureGenerator
    {
       

        public InsertProcedureGenerator(Type type) : base(type)
        {
           
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

                    parameters += sep + "@" + fieldName + " " + typeName;

                    values += sep + "@" + fieldName;

                    sep = ", ";
                }
            });

            var script = $"{snippet} PROCEDURE {ProcedureName} (\n\t{parameters}\n)AS\n";

            script += $"\tINSERT INTO {NameConvention.TableName} ({fields})";

            if (hadId)
            {
                script += "OUTPUT inserted.* ";
            }

            script += $" VALUES ({values})\n";

            if (!hadId)
            {
                script += $"\tDECLARE @newId {idFieldType}=(IDENT_CURRENT('{NameConvention.TableName}'));\n";

                script += $"\tSELECT * FROM {NameConvention.TableName} WHERE {idFieldName}=@newId;\n";
            }

            script += "GO\n\n";

            return script;
        }

        protected override string GetProcedureName()
        {
            return NameConvention.InsertProcedureName;
        }
    }
}