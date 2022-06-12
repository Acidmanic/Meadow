using System;

namespace Meadow.BuildupScripts
{
    public static class ScriptInfoExtensions
    {
        public static string[] SplitScriptIntoBatches(this ScriptInfo info)
        {
            return info.Script.Split(new string[] {"go", "GO", "Go", "gO","--SPLIT"}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}