using System;
using System.IO;
using Acidmanic.Utilities.Results;
using ConsoleAppFramework.Logging;
using Meadow.BuildupScripts;
using Meadow.Log;
using Meadow.Scaffolding.Contracts;
using Meadow.Tools.Assistant.Compilation;

namespace Meadow.Tools.Assistant.Utils
{
    public class ScripUtils
    {
        private readonly ILogger _logger;

        public ScripUtils(ILogger logger)
        {
            _logger = logger;
        }

        public ScripUtils() : this(new NullLogger())
        {
        }
        
        private Result<string> GetValidatedScriptsDirectory(string directory)
        {
            var configurationProviders = new DirectoryCompiler().FastSearchFor<IMeadowConfigurationProvider>(directory);

            if (configurationProviders.Count == 0)
            {
                _logger.Log("No ConfigurationProvider found.");

                return new Result<string>().FailAndDefaultValue();
            }

            _logger.Log(
                $"An instance of {configurationProviders[0].GetType()} is being used to get configurations from.");

            directory = Path.GetFullPath(directory);

            var configurations = configurationProviders[0].GetConfigurations();

            var scriptsDirectory = configurations.BuildupScriptDirectory;

            if (!Path.IsPathRooted(scriptsDirectory))
            {
                scriptsDirectory = Path.Join(directory, scriptsDirectory);
            }

            return new Result<string>().Succeed(scriptsDirectory);
        }

        public Result<ScriptInfo> GetLatestScript(string directory = ".")
        {
            var scriptsDirectory = GetValidatedScriptsDirectory(directory);

            if (scriptsDirectory)
            {
                return GetLatestScriptFromValidScriptsDirectory(scriptsDirectory);
            }

            return new Result<ScriptInfo>().FailAndDefaultValue();
        }

        private Result<ScriptInfo> GetLatestScriptFromValidScriptsDirectory(string validatedScriptsDirectory)
        {
            var scriptManager = new BuildupScriptManager(validatedScriptsDirectory);

            if (scriptManager.ScriptsCount > 0)
            {
                var lastIndex = scriptManager.ScriptsCount - 1;

                return new Result<ScriptInfo>().Succeed(scriptManager[lastIndex]);
            }

            return new Result<ScriptInfo>().FailAndDefaultValue();
        }
        
        public void Blank(
            string title,
            string directory = ".")
        {
            var scriptsDirectory = GetValidatedScriptsDirectory(directory);

            if (scriptsDirectory)
            {
                var latestScript = GetLatestScriptFromValidScriptsDirectory(scriptsDirectory);
                
                var name = ToScriptFileName(title, latestScript ? latestScript.Value.OrderIndex + 1 : 0);

                var scriptPath = Path.Join(scriptsDirectory, name);

                File.Create(scriptPath);

                _logger.Log($"A new blank script file has been created at: {scriptPath}");
            }
            else
            {
                _logger.Log("Unable to find scripts directory.");
            }
        }

        public string ToScriptFileName(string title,int orderIndex =0)
        {
            var currentOrder =  Fix(orderIndex , 4);
            
            var name = currentOrder + "-" + FileNameFriendly(title) + ".sql";

            return name;
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
            string result = value.ToString();

            while (result.Length < digits)
            {
                result = "0" + result;
            }

            return result;
        }
    }
}