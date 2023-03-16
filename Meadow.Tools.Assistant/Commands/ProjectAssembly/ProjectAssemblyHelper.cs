using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Results;
using CoreCommandLine;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Tools.Assistant.DotnetProject;
using Meadow.Tools.Assistant.Utils;
using Microsoft.Extensions.Logging;

namespace Meadow.Tools.Assistant.Commands.ProjectAssembly
{
    public class ProjectAssemblyHelper
    {
        public ProjectAssemblyHelper(ILogger logger, Action<string> output)
        {
            Logger = logger;
            Output = output;
        }

        public ILogger Logger { get; }
        
        public Action<string> Output { get;  }


        public Result<Type> GetMcpType(Context context)
        {
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

                    return providerType;
                }else
                {
                    Logger.LogError("Unable to build the target project.");
                }
            }

            return new Result<Type>().FailAndDefaultValue();
        }

        public Result<MeadowConfiguration> GetMeadowConfiguration(Context context)
        {
            var providerType = GetMcpType(context);

            if (providerType)
            {
                var instance = new ObjectInstantiator().BlindInstantiate(providerType);

                if (instance is IMeadowConfigurationProvider configurationProvider)
                {
                    return new Result<MeadowConfiguration>(true, configurationProvider.GetConfigurations());
                }
            }

            return new Result<MeadowConfiguration>().FailAndDefaultValue();
        }
        public Result<DotnetProjectInfo> TryOpenProject(string projectDirectory)
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
        
        public Result<Type> GetMcpType(Context context, List<Assembly> assemblies)
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
        
        public List<Assembly> LoadAllAssemblies(string directory)
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


        public Result<string> DotnetBuild(string projectDirectory)
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
    }
}