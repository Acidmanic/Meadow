using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Meadow.Tools.Assistant.DotnetProject;
using Meadow.Tools.Assistant.Utils;

namespace Meadow.Tools.Assistant.Compilation
{
    public class CompiledDirectoryResult
    {
        
        
        public List<DotnetProjectInfo> ProjectsOnPath { get; set; }
        
        public List<DotnetProjectInfo> AllIncludedProjects { get; set; }
        
        public List<FileInfo> AllCSharpFiles { get; set; }
        
        public Assembly Assembly { get; set; }
        
    }
}