using System;
using System.IO;
using System.Xml;
using ConsoleAppFramework;
using Meadow.BuildupScripts;
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
            var configurationProviders = new DirectoryCompiler().FastSearchFor<IMeadowConfigurationProvider>(directory);

            if (configurationProviders.Count == 0)
            {
                Console.WriteLine("No ConfigurationProvider found.");

                return;
            }

            Console.WriteLine(
                $"An instance of {configurationProviders[0].GetType()} is being used to get configurations from.");


            directory = Path.GetFullPath(directory);

            var configurations = configurationProviders[0].GetConfigurations();

            var scriptsDirectory = configurations.BuildupScriptDirectory;

            if (!Path.IsPathRooted(scriptsDirectory))
            {
                scriptsDirectory = Path.Join(directory, scriptsDirectory);
            }

            var scriptManager = new BuildupScriptManager(scriptsDirectory);

            var lastIndex = scriptManager.ScriptsCount - 1;

            var lastScript = lastIndex > -1 ? scriptManager[lastIndex] : null;

            var currentOrder = lastScript == null ? "0000" : Fix(lastScript.OrderIndex + 1, 4);

            var name = currentOrder + "-" + FileNameFriendly(title) + ".sql";

            var scriptPath = Path.Join(scriptsDirectory, name);

            File.Create(scriptPath);
        }


        private string FileNameFriendly(string title)
        {
            bool lastDash = false;

            string result = "";

            for (int i = 0; i < title.Length; i++)
            {
                var c = title[i];

                if (char.IsLetterOrDigit(c))
                {
                    result += c;

                    lastDash = false;
                }
                else
                {
                    if (!lastDash && i != 0 && i != title.Length)
                    {
                        lastDash = true;

                        result += "-";
                    }
                }
            }

            return result;
        }

        private string Fix(int value, int digits)
        {
            string result = digits.ToString();

            while (result.Length < digits)
            {
                result = "0" + result;
            }

            return result;
        }
    }
}