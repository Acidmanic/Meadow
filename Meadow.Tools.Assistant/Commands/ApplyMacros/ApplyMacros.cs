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
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Scaffolding.Macros;
using Meadow.Tools.Assistant.Commands.Arguments;
using Meadow.Tools.Assistant.DotnetProject;
using Meadow.Tools.Assistant.Utils;
using Microsoft.Extensions.Logging;

namespace Meadow.Tools.Assistant.Commands.ApplyMacros
{
    [CommandName("apply-macros", "am")]
    [Subcommands(
        typeof(TargetProjectPath),
        typeof(ScriptsDirectory),
        typeof(MeadowConfigurationProvider)
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

                    var providerType = GetMcpType(context, assemblies);

                    if (providerType)
                    {

                        if (PerformApplyingMacros(providerType, assemblies, projectDirectory,
                                context.GetScriptsDirectoryPath()))
                        {
                            Logger.LogInformation("Applied Any found Macros");    
                        }
                    }
                    else
                    {
                        Logger.LogError("Unable to find correct proper implementation of IMeadowConfigurationProvider in target project.");
                    }
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


        private Result<Type> GetMcpType(Context context, List<Assembly> assemblies)
        {
            var providedName = context.GetMeadowConfigurationProvideTypeName();

            var allAvailableClasses = assemblies.ListAllAvailableClasses();

            var configurationProviderTypes = allAvailableClasses
                .Where(c => TypeCheck.Implements<IMeadowConfigurationProvider>(c)
                            && !c.IsAbstract && !c.IsInterface 
                            && c.GetConstructor(new Type[]{})!=null).ToArray();
            
            if (configurationProviderTypes.Length > 0)
            {
                if (string.IsNullOrWhiteSpace(providedName))
                {
                    var providerType = new InteractiveParameter<Type>(
                        configurationProviderTypes, Output, t => t.FullName
                    ).AskFor();

                    return providerType;
                }

                var foundType = configurationProviderTypes
                    .FirstOrDefault(t => providedName.ToLower() == t.FullName?.ToLower());

                if (foundType != null)
                {
                    return foundType;
                }
            }
            else
            {
                Logger.LogError("Target project must contain/reference one implementation of IMeadowConfigurationProvider. ");
            }

            return new Result<Type>().FailAndDefaultValue();
        }


        private Result PerformApplyingMacros(Type providerType, List<Assembly> assemblies, string projectDirectory, string scriptsDir)
        {
            

            var instance = new ObjectInstantiator().BlindInstantiate(providerType);

            if (instance is IMeadowConfigurationProvider configurationProvider)
            {
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

            Logger.LogError(
                "IMeadowConfigurationProvider must be instantiatable by a parameterless constructor.");

            

            return false;
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


        private Result<string> DotnetBuild(string projectDirectory)
        {
            var buildPath = Path.Combine(projectDirectory, "build");

            new GitIgnore(projectDirectory).AppendIfNotExits("build/");

            var startInfo = new ProcessStartInfo
            {
                Arguments = "publish --force --self-contained --output " + buildPath,
                FileName = "dotnet",
                WorkingDirectory = projectDirectory,
                CreateNoWindow = true
            };

            var p = Process.Start(startInfo);

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