using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Results;
using CoreCommandLine;
using CoreCommandLine.Attributes;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Scaffolding.Macros;
using Meadow.Tools.Assistant.Commands.Arguments;
using Meadow.Tools.Assistant.Commands.ProjectAssembly;
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

            var helper = new ProjectAssemblyHelper(Logger, Output);

            var projectInfo = helper.TryOpenProject(projectDirectory);

            if (projectInfo)
            {
                projectDirectory = projectInfo.Value.ProjectFile.Directory!.FullName;

                Logger.LogInformation("Building {Project}", projectInfo.Value.ProjectFile.Name);

                var compiled = helper.DotnetBuild(projectDirectory);

                if (compiled)
                {
                    var assemblies = helper.LoadAllAssemblies(compiled.Value);

                    var providerType = helper.GetMcpType(context, assemblies);

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
                        Logger.LogError(
                            "Unable to find correct proper implementation of IMeadowConfigurationProvider in target project.");
                    }
                }
                else
                {
                    Logger.LogError("Unable to build the target project.");
                }
            }

            return true;
        }


        private Result PerformApplyingMacros(Type providerType, List<Assembly> assemblies, string projectDirectory,
            string scriptsDir)
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

                var engin = new MacroEngine(configurations, assemblies.ToArray());

                engin.ExecuteMacrosFor(scriptsDirectory, f => true);

                return true;
            }

            Logger.LogError(
                "IMeadowConfigurationProvider must be instantiatable by a parameterless constructor.");


            return false;
        }


        public override string Description => "This will update buildup-scripts for their macros in your project.";
    }
}