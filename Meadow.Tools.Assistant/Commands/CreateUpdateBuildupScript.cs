using System;
using ConsoleAppFramework;
using Meadow.Scaffolding;
using Meadow.Tools.Assistant.Compilation;
using Meadow.Tools.Assistant.DotnetProject;
using Meadow.Tools.Assistant.Extensions;
using Meadow.Tools.Assistant.Options;
using Meadow.Tools.Assistant.Utils;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Tools.Assistant.Commands
{
    [Command("create")]
    public class CreateUpdateBuildupScript : ConsoleAppBase
    {
        [Command("script", Descriptions.CreateSqlScript)]
        public void Create(
            [NamespaceOption]
            string @namespace = null,
            [DirectoryOption]
            string directory = ".",
            [Option("p", Descriptions.CreatePolicies)]
            string[] policies = null,
            [NuGetsOption]
            string[] localNuGets = null)
        {
            
            
            var manager = policies.AsPolicyManager();

            var ns = string.IsNullOrEmpty(@namespace)
                ? DotnetProjectInfo.FromDirectory(directory).GetRootNamespace()
                : @namespace;

            var compilationResult = new DirectoryCompiler().WithLocalNuGetDirectory(localNuGets).Compile(directory);

            var availableTypes = compilationResult.Assembly.GetAvailableTypes();

            var script = new AutoScriptGenerator().Generate(directory, availableTypes, ns, manager);

            Console.WriteLine($"Looking up namespace: {ns}");

            script.Save();

            Console.WriteLine(script.Log);
        }

        [Command("blank", Descriptions.CreateBlank)]
        public void Blank(
            [Option("t", "The title you describe your new script with.")]
            string title,
            [DirectoryOption]
            string directory = ".")
        {
            new ScripUtils(new ConsoleLogger()).Blank(title, directory);
        }
    }
}