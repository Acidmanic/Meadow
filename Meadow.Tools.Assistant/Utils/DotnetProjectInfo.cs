using System.IO;
using System.Xml;

namespace Meadow.Tools.Assistant.Utils
{
    public class DotnetProjectInfo
    {
        private readonly DirectoryInfo _directory;
        private readonly FileInfo _projectFile;

        public DotnetProjectInfo(string directory)
        {
            _directory = new DirectoryInfo(directory);

            var projectFile = GetProjectFile(directory);

            if (projectFile == null)
            {
                projectFile = _directory.Name + ".csproj";
            }

            _projectFile = new FileInfo(projectFile);
        }


        public string GetRootNamespace()
        {
            if (_projectFile.Exists)
            {
                return GetRootNamespace(_projectFile.FullName);
            }

            return "";
        }


        private string GetProjectFile(string directory)
        {
            var files = Directory.GetFiles(directory);

            foreach (var file in files)
            {
                if (file.ToLower().EndsWith(".csproj") || file.ToLower().EndsWith(".vbproj"))
                {
                    return file;
                }
            }

            var directories = Directory.EnumerateDirectories(directory);

            foreach (var dir in directories)
            {
                var projFile = GetProjectFile(dir);

                if (projFile != null)
                {
                    return projFile;
                }
            }

            return null;
        }

        private string GetRootNamespace(XmlNode root)
        {
            if (root.Name == "RootNamespace")
            {
                return root.InnerText;
            }

            foreach (XmlNode child in root.ChildNodes)
            {
                var rootNamespace = GetRootNamespace(child);

                if (rootNamespace != null)
                {
                    return rootNamespace;
                }
            }

            return null;
        }

        private string GetRootNamespace(string projectFile)
        {
            if (!string.IsNullOrEmpty(projectFile) && File.Exists(projectFile))
            {
                XmlDocument doc = new XmlDocument();

                var content = File.ReadAllText(projectFile);

                doc.LoadXml(content);

                XmlNode root = doc.FirstChild;

                var projRootNamespace = GetRootNamespace(root);

                if (projRootNamespace != null)
                {
                    return projRootNamespace;
                }

                var file = new FileInfo(projectFile);

                var name = file.Name;

                if (!string.IsNullOrEmpty(file.Extension))
                {
                    name = name.Substring(0, name.Length - file.Extension.Length);
                }

                return name;
            }

            return "";
        }
    }
}