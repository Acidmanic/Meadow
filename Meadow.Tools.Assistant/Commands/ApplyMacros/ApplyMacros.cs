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
using Meadow.Configuration;
using Meadow.Scaffolding.Macros;
using Meadow.Tools.Assistant.Commands.Arguments;
using Meadow.Tools.Assistant.DotnetProject;
using Microsoft.Extensions.Logging;

namespace Meadow.Tools.Assistant.Commands.ApplyMacros
{
    [CommandName("apply-macros", "am")]
    [Subcommands(
        typeof(TargetProjectPath),
        typeof(ScriptsDirectory)
    )]
    public class ApplyMacros : CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            if (!AmIPresent(args))
            {
                return false;
            }

            var projectDirectory = context.GetTargetProjectPath();

            var projectInfo = TryOpenProject(projectDirectory);

            if (projectInfo)
            {
                projectDirectory = projectInfo.Value.ProjectFile.Directory!.FullName;

                Logger.LogInformation("Building {Project}", projectInfo.Value.ProjectFile.Name);

                var compiled = DotnetBuild(projectDirectory);

                if (compiled)
                {
                    var assemblies = LoadAllAssemblies(compiled.Value);

                    PerformApplyingMacros(assemblies, projectDirectory,context.GetScriptsDirectoryPath());

                    Logger.LogInformation("Applied Any found Macros");
                }
                else
                {
                    Logger.LogError("Unable to build the target project.");
                }
            }

            return true;
        }

        private Result<DotnetProjectInfo> TryOpenProject(string projectDirectory)
        {
            try
            {
                projectDirectory = new DirectoryInfo(projectDirectory).FullName;

                var info = DotnetProjectInfo.FromDirectory(projectDirectory);

                return new Result<DotnetProjectInfo>(true, info);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unable to load target project.\n{Exception}", e);
            }

            return new Result<DotnetProjectInfo>().FailAndDefaultValue();
        }


        private Result PerformApplyingMacros(List<Assembly> assemblies, string projectDirectory,string scriptsDir)
        {
            var allAvailableClasses = ListAllAvailableClasses(assemblies);

            var configurationProviderType = allAvailableClasses
                .FirstOrDefault(c => TypeCheck.Implements<IMeadowConfigurationProvider>(c)
                                     && !c.IsAbstract && !c.IsInterface);

            if (configurationProviderType == null)
            {
                Logger.LogError(
                    "Target project must contain/reference one implementation of IMeadowConfigurationProvider");

                return false;
            }

            var configurationProvider = new ObjectInstantiator()
                .BlindInstantiate(configurationProviderType) as IMeadowConfigurationProvider;

            if (configurationProvider == null)
            {
                Logger.LogError("IMeadowConfigurationProvider must be instantiatable by a parameterless constructor.");

                return false;
            }

            var configurations = configurationProvider.GetConfigurations();

            var scriptsDirectory = scriptsDir ?? configurations.BuildupScriptDirectory;

            if (!Path.IsPathFullyQualified(scriptsDirectory))
            {
                scriptsDirectory = Path.Join(projectDirectory, scriptsDirectory);
            }

            var engin = new MacroEngine(assemblies.ToArray());

            engin.ExecuteMacrosFor(scriptsDirectory, f => true);

            return true;
        }

        private List<Type> ListAllAvailableClasses(List<Assembly> assemblies)
        {
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var fuckingTypes = assembly.GetAvailableTypes();

                types.AddRange(fuckingTypes);
            }

            return types;
        }

        private List<Assembly> LoadAllAssemblies(string directory)
        {
            var assemblies = new List<Assembly>();


            LoadAllAssemblies(directory, assemblies);

            return assemblies;
        }

        private void LoadAllAssemblies(string directory, List<Assembly> assemblies)
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
                LoadAllAssemblies(subDirectory.FullName, assemblies);
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