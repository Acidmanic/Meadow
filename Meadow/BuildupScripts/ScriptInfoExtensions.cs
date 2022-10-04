using System;
using System.Text.RegularExpressions;

namespace Meadow.BuildupScripts
{
    public static class ScriptInfoExtensions
    {
        private static readonly Regex Splitter = new Regex("\\s+go\\s+|--SPLIT"); 
        
        public static string[] SplitScriptIntoBatches(this ScriptInfo info)
        {
            return Splitter.Split(info.Script);
        }
    }
}