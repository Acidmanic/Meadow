using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CoreCommandLine;
using CoreCommandLine.Attributes;
using Meadow.Tools.Assistant.DotnetProject;
using Meadow.Tools.Assistant.Utils;
using Microsoft.Extensions.Logging;

namespace Meadow.Tools.Assistant.Commands.ApplyMacros
{
    [CommandName("apply-macros", "-am")]
    public class ApplyMacros : CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            if (!AmIPresent(args))
            {
                return false;
            }
            var projectDirectory = new DirectoryInfo(".").FullName;

            if (args.Length > 1)
            {
                if (Directory.Exists(args[1]))
                {
                    projectDirectory = new DirectoryInfo(args[1]).FullName;
                }
                else if (File.Exists(args[1]) && args[0].ToLower().EndsWith(".csproj"))
                {
                    projectDirectory = new FileInfo(args[0]).Directory?.FullName ?? projectDirectory;
                }
            }

            var projectInfo = DotnetProjectInfo.FromDirectory(projectDirectory);

            if (projectInfo != null)
            {
                
                var lab = new EncapsulatedProject(projectDirectory);
                
                lab.DisableAllEntries();

                var usings = new List<string>{"Meadow.Scaffolding.Macros"};

                var code = "new MacroEngine().ExecuteMacrosFor(args[0],f => true);";
                
                var entry = new MainEntryGenerator()
                    .GenerateMainEntry(usings, lab.Namespace, code);
                
                lab.AddFile(entry.Primary,entry.Secondary+".cs");
                
                Logger.LogInformation("Running {Project}",projectInfo.ProjectFile.Name);
                
                DotnetRun(lab.LaboratoryPath,projectDirectory+"/Scripts");
                
                return true;
            }

            return false;
        }


        private void DotnetRun(string labDirectory, string targetDirectory)
        {
            var startinfo = new ProcessStartInfo
            {
                Arguments = "run " + new DirectoryInfo(labDirectory).FullName,
                FileName = "dotnet",
                WorkingDirectory = labDirectory,
                CreateNoWindow = true
            };
            
            var p = Process.Start(startinfo);
            
            p.WaitForExit();
            
            if (p.ExitCode != 0)
            {
                Logger.LogError("Unable to build the project.");
            }
        }
        
        
        public override string Description => "This will update buildup-scripts for their macros in your project.";
    }
}