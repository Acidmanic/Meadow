using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
using CoreCommandLine;
using CoreCommandLine.Attributes;
using Meadow.Scaffolding.Contracts;
using Meadow.Scaffolding.Macros;
using Meadow.Tools.Assistant.DotnetProject;
using Microsoft.Extensions.Logging;

namespace Meadow.Tools.Assistant.Commands.Macros
{
    [CommandName("apply-macros", "am")]
    public class ApplyMacros : CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            if (!AmIPresent(args))
            {
                return false;
            }

            var projectDirectory = new DirectoryInfo(".").FullName;

            if (args.Length > 1)
            {
                if (Directory.Exists(args[1]))
                {
                    projectDirectory = new DirectoryInfo(args[1]).FullName;
                }
                else if (File.Exists(args[1]) && args[0].ToLower().EndsWith(".csproj"))
                {
                    projectDirectory = new FileInfo(args[0]).Directory?.FullName ?? projectDirectory;
                }
            }

            var projectInfo = DotnetProjectInfo.FromDirectory(projectDirectory);

            Logger.LogInformation("Build {Project}", projectInfo.ProjectFile.Name);

            var compiled = DotnetBuild(projectDirectory);

            if (compiled)
            {
                var assemblies = LoadEveryFuckingThing(compiled.Value);

                GoNuts(assemblies, projectDirectory);
                
                Console.WriteLine("meeh");
            }

            return false;
        }

        private void GoNuts(List<Assembly> assemblies, string projectDirectory)
        {
            var everyFuckingClass = GetEveryFuckingClass(assemblies);

            var meadowConfigurationProvider = everyFuckingClass
                .FirstOrDefault(c => TypeCheck.Implements<IMeadowConfigurationProvider>(c)
                                     && !c.IsAbstract && !c.IsInterface);

            var provider = new ObjectInstantiator()
                .BlindInstantiate(meadowConfigurationProvider) as IMeadowConfigurationProvider;

            var configurations = provider.GetConfigurations();

            var scriptsDirectory = configurations.BuildupScriptDirectory;

            if (!Path.IsPathFullyQualified(scriptsDirectory))
            {
                scriptsDirectory = Path.Join(projectDirectory,scriptsDirectory) ;   
            }

            var engin = new MacroEngine(assemblies.ToArray());
            
            engin.ExecuteMacrosFor(scriptsDirectory,f => true);

        }

        private List<Type> GetEveryFuckingClass(List<Assembly> assemblies)
        {
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var fuckingTypes = assembly.GetAvailableTypes();
                
                types.AddRange(fuckingTypes);
            }

            return types;
        }

        private List<Assembly> LoadEveryFuckingThing(string directory)
        {
            var assemblies = new List<Assembly>();


            LoadEveryFuckingThing(directory, assemblies);

            return assemblies;
        }

        private void LoadEveryFuckingThing(string directory, List<Assembly> assemblies)
        {
            var files = new DirectoryInfo(directory).GetFiles();

            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file.FullName);

                    assemblies.Add(assembly);
                }
                catch (Exception _)
                {
                }
            }

            var directories = new DirectoryInfo(directory).GetDirectories();

            foreach (var subDirectory in directories)
            {
                LoadEveryFuckingThing(subDirectory.FullName, assemblies);
            }
        }


        private void DotnetRun(string labDirectory, string targetDirectory)
        {
            var startinfo = new ProcessStartInfo
            {
                Arguments = "run " + new DirectoryInfo(labDirectory).FullName,
                FileName = "dotnet",
                WorkingDirectory = labDirectory,
                CreateNoWindow = true
            };

            var p = Process.Start(startinfo);

            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                Logger.LogError("Unable to build the project.");
            }
        }

        private Result<string> DotnetBuild(string projectDirectory)
        {
            var buildPath = Path.Combine(projectDirectory, "build");

            var startinfo = new ProcessStartInfo
            {
                Arguments = "build --output " + buildPath,
                FileName = "dotnet",
                WorkingDirectory = projectDirectory,
                CreateNoWindow = true
            };

            var p = Process.Start(startinfo);

            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                Logger.LogError("Unable to build the project.");

                return new Result<string>().FailAndDefaultValue();
            }

            return new Result<string>(true, buildPath);
        }

        public override string Description => "This will update buildup-scripts for their macros in your project.";
    }
}