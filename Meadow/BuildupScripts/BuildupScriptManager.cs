using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Meadow.Configuration;
using Meadow.Exceptions;
using Meadow.Scaffolding.Macros;

namespace Meadow.BuildupScripts
{
    public class BuildupScriptManager
    {
        private readonly DirectoryInfo _directory;
        private readonly List<ScriptInfo> _scripts;


        public BuildupScriptManager(string directory, MacroPolicies macroPolicy,
            params Assembly[] macroContainingAssemblies)
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();

            if (Path.IsPathFullyQualified(directory))
            {
                _directory = new DirectoryInfo(directory);
            }
            else
            {
                if (entryAssembly != null)
                {
                    var path = new FileInfo(entryAssembly.Location).Directory?.FullName;

                    if (path != null)
                    {
                        path = Path.Combine(path ?? "", directory);

                        _directory = new DirectoryInfo(path);
                    }
                }

                if (_directory == null)
                {
                    //TODO: Warn
                    _directory = new DirectoryInfo(directory);
                }

                if (!_directory.Exists)
                {
                    throw new MissingDirectoryScriptsException();
                }
            }

            _scripts = new List<ScriptInfo>();

            Update(macroPolicy, macroContainingAssemblies);
        }


        public void Update(MacroPolicies macroPolicy, params Assembly[] macroContainingAssemblies)
        {
            var macroEngine = new MacroEngine(macroContainingAssemblies);

            _scripts.Clear();

            var files = _directory.EnumerateFiles().ToArray();

            foreach (var file in files)
            {
                var name = file.Name;

                var result = IsValidBuildupScriptName(name);

                if (result.IsValid)
                {
                    try
                    {
                        macroEngine.ExecuteMacrosFor(file, (hadMacro, content) =>
                        {
                            if (hadMacro && macroPolicy == MacroPolicies.Ignore)
                            {
                                //Revert
                                content = File.ReadAllText(file.FullName);
                            }

                            content = Normalize(content);

                            var scriptInfo = new ScriptInfo
                            {
                                Name = result.Name,
                                Script = content,
                                OrderIndex = result.OrderIndex,
                                Order = result.Order,
                                ScriptFile = file
                            };

                            _scripts.Add(scriptInfo);

                            return hadMacro && macroPolicy == MacroPolicies.UpdateScripts;
                        });
                    }
                    finally
                    {
                    }
                }
            }

            _scripts.Sort(new ScriptInfoAscendingComparer());
        }

        private string Normalize(string content)
        {
            content = content.Replace('\r', '\n');

            while (content.IndexOf("\n\n", StringComparison.Ordinal) > -1)
            {
                content = content.Replace("\n\n", "\n");
            }

            content = content.Replace("\n", "\r\n");

            return content;
        }

        private BsnCheckResult IsValidBuildupScriptName(string name)
        {
            var falseResult = new BsnCheckResult
            {
                IsValid = false
            };
            // ####<>.sql
            if (name == null || name.Length < 9)
            {
                return falseResult;
            }

            var extension = name.Substring(name.Length - 4, 4);

            if (extension.ToLower() != ".sql")
            {
                return falseResult;
            }

            var starter = name.Substring(0, 4);

            if (!Int32.TryParse(starter, out var order))
            {
                return falseResult;
            }

            var title = name.Substring(4, name.Length - 8);

            title = title.Trim('-', '_', ' ', '\n', '\r', '\t', ',', '.', ';', ':');

            return new BsnCheckResult
            {
                Name = title,
                IsValid = true,
                OrderIndex = order,
                Order = starter
            };
        }

        public int ScriptsCount => _scripts.Count;

        public ScriptInfo this[int index] => _scripts[index];
    }
}