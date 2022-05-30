using System;
using Meadow.Reflection.Conventions;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    public class DeleteProcedureGenerator : ProcedureGenerator
    {
        public bool ById { get; }

        public DeleteProcedureGenerator(Type type, bool byId) : base(type)
        {
            ById = byId;
        }

        protected override string GenerateScript(SqlScriptActions action, string snippet)
        {
            var idField = GetIdField(Type);

            var useIdField = ById && idField != null;

            var parameters = useIdField ? $"@{idField.Name} {TypeNameMapper[idField.Type]}" : "";

            var script = $"{snippet} PROCEDURE {ProcedureName}({parameters})\nAS";

            var where = useIdField ? $" WHERE {idField.Name}=@{idField.Name}" : "";

            script += $"\n\tDECLARE @existing = (SELECT COUNT(*) FROM {NameConvention.TableName});";

            script += $"\n\tDELETE FROM {NameConvention.TableName}{where}";

            script += $"\n\tDECLARE @delta = @existing - (SELECT COUNT(*) FROM {NameConvention.TableName});";

            script += "\n\tIF @delta > 0 or @existing = 0\n\t\tSELECT cast(1 as bit) Success";

            script += "\n\tELSE\n\t\tselect cast(0 as bit) Success\nGO\n\n";

            return script;
        }

        protected override string GetProcedureName()
        {
            return ById
                ? NameConvention.DeleteByIdProcedureName
                : NameConvention.DeleteAllProcedureName;
        }
    }
}