using System;
using System.IO;
using ConsoleAppFramework;
using Meadow.Scaffolding;
using Meadow.Scaffolding.OnExistsPolicy;

namespace Meadow.Tools.Assistant.Commands
{

    [Command("create")]
    public class CreateUpdateBuildupScript : ConsoleAppBase
    {
        
        [Command("script", Descriptions.Create)]
        public void Create(
            [Option("ns", "Root namespace to search for models.")]
            string @namespace = "",
            [Option("d", "The path to target Meadow Project")]
            string directory = ".",
            [Option("p", Descriptions.CreatePolicies)]
            string[] policies = null)
        {
            var manager = policies.AsPolicyManager();


            var script = new AutoScriptGenerator().Generate(directory, @namespace, manager);


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

            Console.WriteLine(script.Log);
        }
    }
}