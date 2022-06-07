using System.IO;
using Meadow.BuildupScripts;
using Meadow.Scaffolding;

namespace Meadow.Tools.Assistant.Extensions
{
    public static class ScriptGeneratingResultExtensions
    {

        public static void Save(this AutoScriptGenerator.ScriptGeneratingResult script)
        {
            if (script.Created)
            {
                var dir = script.ScriptInfo.ScriptFile.Directory;

                if (!dir.Exists)
                {
                    dir.Create();
                }

                var path = script.ScriptInfo.ScriptFile.FullName;

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                File.WriteAllText(path, script.ScriptInfo.Script);
            }
        }     
        
    }
}