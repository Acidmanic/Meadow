using System.IO;

namespace Meadow.BuildupScripts
{
    public class ScriptInfo
    {
        public FileInfo ScriptFile { get; set; }

        public string Name { get; set; }

        public int OrderIndex { get; set; }

        public string Script { get; set; }
    }
}