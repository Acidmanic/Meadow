using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Meadow.BuildupScripts
{
    public static class ScriptInfoExtensions
    {
        private static readonly Regex Splitter = new Regex("\\s+go\\s+|--SPLIT"); 
        
        public static string[] SplitScriptIntoBatches(this ScriptInfo info)
        {
            var splitedScripts =  Splitter.Split(" " + info.Script + " ");

            var list = new List<string>();

            foreach (var script in splitedScripts)
            {
                var trimmed = script.Trim();

                if (!string.IsNullOrEmpty(trimmed))
                {
                    list.Add(trimmed);
                }
                
            }

            return list.ToArray();
        }
    }
}