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
    public class ListFieldsNames : ConsoleAppBase
    {
        [Command("fields", "This will list any found model on the given directory")]
        public void ListModels(
            [ModelNameOptions] string modelName,
            [DirectoryOption] string directory = ".",
            [NamespaceOption] string @namespace = "",
            [NuGetsOption] string[] localNuGets = null,
            [FullDetailsOption] bool fullTree = false)
        {
            var ns = string.IsNullOrEmpty(@namespace)
                ? DotnetProjectInfo.FromDirectory(directory).GetRootNamespace()
                : @namespace;

            var compilationResult = new DirectoryCompiler().WithLocalNuGetDirectory(localNuGets).Compile(directory);

            var foundModels = compilationResult.Assembly.GetAvailableTypes()
                .Where(t => t.Name == modelName || t.FullName == modelName);


            Console.WriteLine("Found these Models:");

            foreach (var modelType in foundModels)
            {
                Console.WriteLine("----------------------------");
                Console.WriteLine(modelType.FullName + "\t\t" + (fullTree ? "FULL-TREE" : "ROOT-CHILDREN"));
                Console.WriteLine("----------------------------");


                var translator = new RelationalFieldAddressIdentifierTranslator() {Separator = "_"};

                var map = translator.MapAddressesByIdentifier(modelType,fullTree);

                foreach (var item in map)
                {
                    Console.WriteLine(item.Key);
                }
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