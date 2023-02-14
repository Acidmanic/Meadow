using System;

namespace Meadow.SqlServer.SqlScriptsGenerators
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

            var useIdField = ById && HasIdField;

            var parameters = useIdField ? $"(@{IdField.Name} {TypeNameMapper[IdField.Type]})" : "";

            var script = $"{snippet} PROCEDURE {ProcedureName}{parameters}\nAS";

            var where = useIdField ? $" WHERE {IdField.Name}=@{IdField.Name}" : "";

            script += $"\n\tDECLARE @existing int = (SELECT COUNT(*) FROM {NameConvention.TableName});";

            script += $"\n\tDELETE FROM {NameConvention.TableName}{where}";

            script += $"\n\tDECLARE @delta int = @existing - (SELECT COUNT(*) FROM {NameConvention.TableName});";

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