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
            [DelimiterOptions] char delimiter='_',
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


                var translator = new RelationalRelationalIdentifierToStandardFieldMapper() {Separator = delimiter};

                var map = translator.MapAddressesByIdentifier(modelType,fullTree);

                foreach (var item in map)
                {
                    Console.WriteLine(item.Key);
                }
            }
        }

    }
}