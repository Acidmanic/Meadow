using System;
using System.IO;
using System.Xml;
using ConsoleAppFramework;
using Meadow.Scaffolding;
using Meadow.Scaffolding.OnExistsPolicy;
using Meadow.Tools.Assistant.Utils;

namespace Meadow.Tools.Assistant.Commands
{
    [Command("create")]
    public class CreateUpdateBuildupScript : ConsoleAppBase
    {
        [Command("script", Descriptions.Create)]
        public void Create(
            [Option("ns",
                "Root namespace to search for models. The default value would be RootNamespace of the project.")]
            string @namespace = null,
            [Option("d", "The path to target Meadow Project")]
            string directory = ".",
            [Option("p", Descriptions.CreatePolicies)]
            string[] policies = null)
        {
            var manager = policies.AsPolicyManager();

            var ns = string.IsNullOrEmpty(@namespace) ? new DotnetProjectInfo(directory).GetRootNamespace() : @namespace;

            var script = new AutoScriptGenerator().Generate(directory, ns, manager);


            Console.WriteLine($"Looking up namespace: {ns}");

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