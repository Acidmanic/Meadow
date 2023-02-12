using System;
using System.IO;
using System.Text.RegularExpressions;
using Meadow.Tools.Assistant.DotnetProject;

namespace Meadow.Tools.Assistant.Utils
{
    /// <summary>
    /// This class helps to copy a project to an encapsulated directory and manipulate it
    /// before being built 
    /// </summary>
    public class EncapsulatedProject : IDisposable
    {
        public string LaboratoryPath { get; private set; }

        public string Namespace => _projectInfo.GetRootNamespace();


        private readonly DotnetProjectInfo _projectInfo;

        public EncapsulatedProject(string projectDirectory)
        {
            
            LaboratoryPath = projectDirectory + ".mat-laboratory.4c337b76aadb11ed8097eb38668575d7";

            if (Directory.Exists(LaboratoryPath))
            {
                Directory.Delete(LaboratoryPath, true);
            }

            Directory.CreateDirectory(LaboratoryPath);

            CopyContentRecursive(projectDirectory, LaboratoryPath);

            _projectInfo = new DotnetProjectInfo(LaboratoryPath);
        }


        public void AddFile(string content, string filename)
        {
            var destinationFile = Path.Join(LaboratoryPath, filename);

            if (File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
            }

            File.WriteAllText(destinationFile, content);
        }

        public void DisableAllEntries()
        {
            var sources = _projectInfo.GetSourceCodes();

            foreach (var source in sources)
            {
                var content = File.ReadAllText(source.FullName);

                content = DisableEntry(content);

                File.Delete(source.FullName);

                File.WriteAllText(source.FullName, content);
            }
        }

        private string DisableEntry(string content)
        {
            var pattern = "void\\s+Main\\s*\\(\\s*(params)?\\s*string\\s*\\[\\]\\s*args\\s*\\)";

            var reg = new Regex(pattern);

            var goOn = true;

            while (goOn)
            {
                goOn = false;

                var matches = reg.Matches(content);

                if (matches.Count > 0)
                {
                    goOn = true;

                    var firstMatch = matches[0];

                    var pre = content.Substring(0, firstMatch.Index);
                    var passedLength = firstMatch.Index + firstMatch.Length;
                    var post = content.Substring(passedLength, content.Length - passedLength);

                    content = pre + "void _Main_Disabled_(string[] args)" + post;
                }
            }

            return content;
        }

        private void CopyContentRecursive(string source, string destination)
        {
            var files = Directory.GetFiles(source);

            foreach (var filePath in files)
            {
                var fileName = new FileInfo(filePath).Name;

                var destinationFile = Path.Combine(destination, fileName);

                File.Copy(filePath, destinationFile);
            }

            var directories = Directory.GetDirectories(source);

            foreach (var directoryPath in directories)
            {
                var directoryName = new DirectoryInfo(directoryPath).Name;

                var destinationDirectory = Path.Join(destination, directoryName);

                Directory.CreateDirectory(destinationDirectory);

                CopyContentRecursive(directoryPath, destinationDirectory);
            }
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(LaboratoryPath))
                {
                    Directory.Delete(LaboratoryPath,true);
                }
            }
            catch (Exception e)
            {
                
            }
        }
    }
}