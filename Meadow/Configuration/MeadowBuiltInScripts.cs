using System.Collections.Generic;
using System.IO;
using Meadow.BuildupScripts;
using Meadow.Configuration.ConfigurationRequests.Models;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.SqlScriptsGenerators;

namespace Meadow.Configuration
{
    public class MeadowBuiltInScripts
    {
        public List<ScriptInfo> GenerateHistoryBasis()
        {
            var type = typeof(MeadowDatabaseHistory);

            var result = new List<ScriptInfo>(); 

            var script = new TableScriptGenerator(type).Generate().Text;
            
            result.Add(Info(script));

            script = new InsertProcedureGenerator(type).Generate().Text;

            script += new ReadSequenceProcedureGenerator(type, false, 1, false).Generate().Text;

            result.Add(Info(script));

            return result;
        }

        private ScriptInfo Info(string sql)
        {
            return new ScriptInfo
            {
                Name = "MeadowHistory",
                Order = "0",
                Script = sql,
                OrderIndex = 0, ScriptFile = new FileInfo("temp.sql")
            };
        }
    }
}