using System;
using System.Collections.Generic;
using System.IO;

namespace Meadow.BuildupScripts
{
    public class BuildupScriptManager
    {
        private readonly DirectoryInfo _directory;
        private readonly List<ScriptInfo> _scripts;


        public BuildupScriptManager(string directory)
        {
            _directory = new DirectoryInfo(directory);
            _scripts = new List<ScriptInfo>();
            Update();
        }


        public void Update()
        {
            var files = _directory.EnumerateFiles();
            _scripts.Clear();
            foreach (var file in files)
            {
                var name = file.Name;
                var result = IsValidBuildupScriptName(name);

                if (result.IsValid)
                {
                    try
                    {
                        var content = File.ReadAllText(file.FullName);

                        var scriptInfo = new ScriptInfo
                        {
                            Name = result.Name,
                            Script = content,
                            OrderIndex = result.OrderIndex,
                            ScriptFile = file
                        };
                        _scripts.Add(scriptInfo);
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }
            }
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

            return new BsnCheckResult
            {
                Name = title,
                IsValid = true,
                OrderIndex = order
            };
        }

        public int ScriptsCount => _scripts.Count;

        public ScriptInfo this[int index] => _scripts[index];
    }
}