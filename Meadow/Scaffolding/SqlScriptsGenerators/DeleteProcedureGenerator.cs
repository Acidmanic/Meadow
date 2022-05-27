using System;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    public class DeleteProcedureGenerator:ProcedureGenerator
    {
        public bool ById { get; }
        
        public DeleteProcedureGenerator(Type type, bool byId) : base(type)
        {
            ById = byId;
        }

        protected override string GenerateScript(bool alreadyExists, string createKeyword)
        {
            var idField = GetIdField(Type);
            
            var parameters = ById ? $"@{idField.Name} {TypeNameMapper[idField.Type]}" : "";

            var script = $"{createKeyword} PROCEDURE {ProcedureName}({parameters})\nAS";

            var where = ById ? $" WHERE {idField.Name}=@{idField.Name}" : "";

            script += $"\n\tDECLARE @existing = (SELECT COUNT(*) FROM {TableName});";

            script += $"\n\tDELETE FROM {TableName}{where}";

            script += $"\n\tDECLARE @delta = @existing - (SELECT COUNT(*) FROM {TableName});";

            script += "\n\tIF @delta > 0 or @existing = 0\n\t\tSELECT cast(1 as bit) Success";

            script += "\n\tELSE\n\t\tselect cast(0 as bit) Success\nGO\n\n";

            return script;
        }

        protected override string GetProcedureName()
        {
            var spName = "spDelete" + (ById ? $"{EntityName}ById" : $"All{TableName}");

            return spName;
        }
    }
}