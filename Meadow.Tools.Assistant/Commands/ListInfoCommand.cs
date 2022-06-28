using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using ConsoleAppFramework;
using Meadow.Reflection.FetchPlug;
using Meadow.Tools.Assistant.Compilation;
using Meadow.Tools.Assistant.DotnetProject;
using Meadow.Tools.Assistant.Extensions;
using Meadow.Tools.Assistant.Options;

namespace Meadow.Tools.Assistant.Commands
{
    [Command("list")]
    public class ListInfoCommand : ConsoleAppBase
    {
        [Command("models", "This will list any found model on the given directory")]
        public void ListModels(
            [DirectoryOption] string directory,
            [NamespaceOption] string @namespace = "",
            [NuGetsOption] string[] localNuGets = null,
            [FullDetailsOption] bool fullDetails = false)
        {
            var ns = string.IsNullOrEmpty(@namespace)
                ? DotnetProjectInfo.FromDirectory(directory).GetRootNamespace()
                : @namespace;

            var compilationResult = new DirectoryCompiler().WithLocalNuGetDirectory(localNuGets).Compile(directory);

            var availableTypes = compilationResult.Assembly.GetAvailableTypes();

            if (fullDetails)
            {
                LongAnswer(availableTypes, ns);
            }
            else
            {
                ShortAnswer(availableTypes, ns);
            }
        }

        private void LongAnswer(List<Type> availableTypes, string ns)
        {
            var ta = new TypeAcquirer();

            Console.WriteLine("-----------------------");
            Console.WriteLine("Model?\tNs-Match?\tType");
            Console.WriteLine("-----------------------");
            foreach (var type in availableTypes)
            {
                var isModel = TypeCheck.IsModel(type);
                var isUnderNamespace = ta.NamespaceMatch(ns, type.Namespace);

                Console.WriteLine($"{(isModel ? "YES" : "NO")}\t{(isUnderNamespace ? "YES" : "NO")}\t{type.FullName}");
            }
        }

        private void ShortAnswer(List<Type> availableTypes, string ns)
        {
            Console.WriteLine($"Found {availableTypes.Count} Types available.");

            var models = new TypeAcquirer().EnumerateModels(availableTypes, "").ToList();

            Console.WriteLine($"Found {models.Count()} Models.");

            var filteredModels = new TypeAcquirer().EnumerateModels(models, ns).ToList();

            Console.WriteLine($"Found {filteredModels.Count()} Models under namespace: '{ns}':");

            foreach (var modelType in filteredModels)
            {
                Console.WriteLine(modelType.Name);
            }
        }
    }
}