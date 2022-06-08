using System;
using System.IO;
using System.Xml;
using ConsoleAppFramework;
using Meadow.BuildupScripts;
using Meadow.Log;
using Meadow.Scaffolding;
using Meadow.Scaffolding.Contracts;
using Meadow.Scaffolding.OnExistsPolicy;
using Meadow.Tools.Assistant.Compilation;
using Meadow.Tools.Assistant.DotnetProject;
using Meadow.Tools.Assistant.Extensions;
using Meadow.Tools.Assistant.Utils;

namespace Meadow.Tools.Assistant.Commands
{
    [Command("create")]
    public class CreateUpdateBuildupScript : ConsoleAppBase
    {
        [Command("script", Descriptions.CreateSqlScript)]
        public void Create(
            [Option("ns",
                "Root namespace to search for models. The default value would be RootNamespace of the project.")]
            string @namespace = null,
            [Option("d", "The path to target Meadow Project")]
            string directory = ".",
            [Option("p", Descriptions.CreatePolicies)]
            string[] policies = null,
            [Option("n", "comma separated List of Local nuget directories")]
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
            [Option("d", "The path to target Meadow Project")]
            string directory = ".")
        {
            new ScripUtils(new ConsoleLogger()).Blank(title, directory);
        }
    }
}