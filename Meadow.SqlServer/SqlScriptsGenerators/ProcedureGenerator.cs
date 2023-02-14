using System;

namespace Meadow.SqlServer.SqlScriptsGenerators
{
    public abstract class ProcedureGenerator : SqlGeneratorBaseLegacy
    {
        protected ProcedureGenerator(Type type) : base(type)
        {
        }

        public string ProcedureName => GetProcedureName();

        public override string SqlObjectName => ProcedureName;

        public override DbObjectTypes ObjectType => DbObjectTypes.StoredProcedures;

        protected abstract string GetProcedureName();


        public override Code Generate(SqlScriptActions action)
        {
            var snippet = "CREATE ";
            if (action == SqlScriptActions.Alter)
            {
                snippet = "ALTER ";
            }
            else if(action==SqlScriptActions.DropCreate)
            {
                snippet = $"DROP PROCEDURE IF EXISTS {GetProcedureName()}\n\nCREATE ";
            }
            var script = GenerateScript(action, snippet);

            return new Code
                {
                    Name = ProcedureName,
                    Text = script
                };
        }

        protected abstract string GenerateScript(SqlScriptActions action,string preProcedureSnippet);
    }
}